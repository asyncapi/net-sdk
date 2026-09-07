// Copyright © 2021-Present Neuroglia SRL. All rights reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License"),
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System.Text;
using Json.Schema;
using Neuroglia.AsyncApi.AspNetCore.UI.Models.v3;
using Neuroglia.AsyncApi.IO;
using Neuroglia.AsyncApi.v3;
using Neuroglia.Serialization;

namespace Neuroglia.AsyncApi.UnitTests.Cases.UI;

public class V3SchemaRenderingTests
{
    public V3SchemaRenderingTests()
    {
        var services = new ServiceCollection();
        services.AddAsyncApiIO();
        services.AddSerialization().AddJsonSerializer();
        this.ServiceProvider = services.BuildServiceProvider();
    }

    protected ServiceProvider ServiceProvider { get; }
    protected IAsyncApiDocumentReader DocumentReader => this.ServiceProvider.GetRequiredService<IAsyncApiDocumentReader>();
    protected IJsonSerializer JsonSerializer => this.ServiceProvider.GetRequiredService<IJsonSerializer>();

    private const string SampleAsyncApiDocumentJson = """
    {
      "asyncapi": "3.0.0",
      "info": {
        "title": "Sample API",
        "version": "1.0.0"
      },
      "channels": {
        "testChannel": {
          "address": "test",
          "messages": {
            "testMessage": {
              "payload": {
                "$ref": "#/components/schemas/SampleMessage"
              }
            }
          }
        }
      },
      "components": {
        "schemas": {
          "SampleMessage": {
            "schemaFormat": "application/vnd.aai.asyncapi+json;version=3.0.0",
            "schema": {
              "type": "object",
              "properties": {
                "title": { "type": "string" },
                "sub": { "type": "integer" }
              },
              "required": ["sub"]
            }
          }
        }
      }
    }
    """;

    private async Task<V3AsyncApiDocument> LoadSampleDocumentAsync()
    {
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(SampleAsyncApiDocumentJson));
        var document = await this.DocumentReader.ReadAsync(stream);
        document.Should().NotBeNull();
        return (V3AsyncApiDocument)document!;
    }

    /// <summary>
    /// Simulates schema resolution in _V3Schema.cshtml (uses Model.Definition.Schema).
    /// </summary>
    static JsonSchema ResolveJsonSchema(V3SchemaViewModel model, IJsonSerializer serializer)
    {
        return model.Definition.Schema is JsonSchema jsonSchema
            ? jsonSchema
            : serializer.Deserialize<JsonSchema>(serializer.SerializeToText(model.Definition.Schema))!;
    }

    /// <summary>
    /// Simulates the old buggy schema resolution in _V3Schema.cshtml (used Model.Definition).
    /// </summary>
    static JsonSchema ResolveJsonSchemaOld(V3SchemaViewModel model, IJsonSerializer serializer)
    {
        return model.Definition.Schema is JsonSchema jsonSchema
            ? jsonSchema
            : serializer.Deserialize<JsonSchema>(serializer.SerializeToText(model.Definition))!;
    }

    [Fact]
    public async Task Old_Resolution_Demonstrates_Issue_1189_Failure()
    {
        // arrange: component schema from GitHub issue #1189
        var document = await this.LoadSampleDocumentAsync();
        var definition = document.Components!.Schemas!["SampleMessage"];
        var viewModel = new V3SchemaViewModel(document, SchemaContext.Unknown, definition, "#/components/schemas/SampleMessage");

        // act: old resolution serializing Model.Definition
        var oldSchema = ResolveJsonSchemaOld(viewModel, this.JsonSerializer);

        // assert: demonstrates issue #1189: top-level type is null and properties are null
        oldSchema.Should().NotBeNull();
        oldSchema.GetJsonType().Should().BeNull();
        oldSchema.GetProperties().Should().BeNull();
    }

    [Fact]
    public async Task Component_Object_Schema_Properties_Should_Resolve()
    {
        // arrange: component schema with multiple properties and required field
        var document = await this.LoadSampleDocumentAsync();
        var definition = document.Components!.Schemas!["SampleMessage"];
        var viewModel = new V3SchemaViewModel(document, SchemaContext.Unknown, definition, "#/components/schemas/SampleMessage");

        // act: fixed resolution serializing Model.Definition.Schema
        var schema = ResolveJsonSchema(viewModel, this.JsonSerializer);

        // assert: object type, properties and required fields are correctly resolved
        schema.Should().NotBeNull();
        schema.GetJsonType().Should().Be(SchemaValueType.Object);

        var properties = schema.GetProperties();
        properties.Should().NotBeNull().And.HaveCount(2);
        properties!.Should().ContainKey("title");
        properties!["title"].GetJsonType().Should().Be(SchemaValueType.String);
        properties!.Should().ContainKey("sub");
        properties!["sub"].GetJsonType().Should().Be(SchemaValueType.Integer);

        var required = schema.GetRequired();
        required.Should().NotBeNull().And.Contain("sub");
    }

    [Fact]
    public async Task Referenced_Payload_Schema_Properties_Should_Resolve()
    {
        // arrange: payload referencing #/components/schemas/SampleMessage
        var document = await this.LoadSampleDocumentAsync();
        var message = document.Channels!["testChannel"].Messages!["testMessage"];
        message.Payload.Should().NotBeNull();

        var dereferencedDefinition = document.DereferenceSchema(message.Payload!.Reference!);
        var viewModel = new V3SchemaViewModel(document, SchemaContext.Payload, dereferencedDefinition, "#/components/schemas/SampleMessage");

        // act
        var schema = ResolveJsonSchema(viewModel, this.JsonSerializer);

        // assert: referenced schema properties are accessible
        schema.Should().NotBeNull();
        schema.GetJsonType().Should().Be(SchemaValueType.Object);

        var properties = schema.GetProperties();
        properties.Should().NotBeNull().And.HaveCount(2);
        properties!.Should().ContainKeys("title", "sub");
        schema.GetRequired().Should().NotBeNull().And.Contain("sub");
    }

    [Fact]
    public void Prebuilt_JsonSchema_Instance_Should_Be_Preserved()
    {
        // arrange: definition wrapping an existing JsonSchema instance
        var prebuiltSchema = new JsonSchemaBuilder()
            .Type(SchemaValueType.Object)
            .Properties(("title", new JsonSchemaBuilder().Type(SchemaValueType.String).Build()))
            .Build();

        var definition = new V3SchemaDefinition
        {
            SchemaFormat = "application/vnd.aai.asyncapi+json;version=3.0.0",
            Schema = prebuiltSchema
        };
        var viewModel = new V3SchemaViewModel(new V3AsyncApiDocument(), SchemaContext.Unknown, definition, "prebuilt");

        // act
        var schema = ResolveJsonSchema(viewModel, this.JsonSerializer);

        // assert: prebuilt schema is directly preserved without re-serialization
        schema.Should().NotBeNull().And.BeSameAs(prebuiltSchema);
        schema.GetProperties().Should().ContainKey("title");
    }
}
