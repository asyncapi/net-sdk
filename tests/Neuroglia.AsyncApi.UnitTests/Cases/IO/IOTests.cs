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

using Neuroglia.AsyncApi.IO;
using Neuroglia.AsyncApi.Bindings.Mqtt;
using Neuroglia.AsyncApi.v3;

namespace Neuroglia.AsyncApi.UnitTests.Cases.IO;

public class IOTests
{

    public IOTests()
    {
        var services = new ServiceCollection();
        services.AddAsyncApiIO();
        this.ServiceProvider = services.BuildServiceProvider();
    }

    protected ServiceProvider ServiceProvider { get; }

    protected IAsyncApiDocumentWriter DocumentWriter => this.ServiceProvider.GetRequiredService<IAsyncApiDocumentWriter>();

    protected IAsyncApiDocumentReader DocumentReader => this.ServiceProvider.GetRequiredService<IAsyncApiDocumentReader>();

    [Fact]
    public async Task Write_Then_Read_Json_Document_V2_Should_Work()
    {
        //arrange
        var documentToWrite = AsyncApiDocumentFactory.CreateV2();
        using var stream = new MemoryStream();

        //act
        await this.DocumentWriter.WriteAsync(documentToWrite, stream, AsyncApiDocumentFormat.Json);
        await stream.FlushAsync();
        stream.Position = 0;
        var readDocument = await this.DocumentReader.ReadAsync(stream);

        //assert
        readDocument.Should().BeEquivalentTo(documentToWrite);
    }

    [Fact]
    public async Task Write_Then_Read_Yaml_Document_V2_Should_Work()
    {
        //arrange
        var documentToWrite = AsyncApiDocumentFactory.CreateV2();
        using var stream = new MemoryStream();
        
        //act
        await this.DocumentWriter.WriteAsync(documentToWrite, stream, AsyncApiDocumentFormat.Yaml);
        await stream.FlushAsync();
        stream.Position = 0;
        var readDocument = await this.DocumentReader.ReadAsync(stream);

        //assert
        readDocument.Should().BeEquivalentTo(documentToWrite);
    }

    [Fact]
    public async Task Write_Then_Read_Json_Document_V3_Should_Work()
    {
        //arrange
        var documentToWrite = AsyncApiDocumentFactory.CreateV3();
        using var stream = new MemoryStream();

        //act
        await this.DocumentWriter.WriteAsync(documentToWrite, stream, AsyncApiDocumentFormat.Json);
        await stream.FlushAsync();
        stream.Position = 0;
        var readDocument = await this.DocumentReader.ReadAsync(stream);

        //assert
        readDocument.Should().BeEquivalentTo(documentToWrite);
    }

    [Fact]
    public async Task Write_Then_Read_Yaml_Document_V3_Should_Work()
    {
        //arrange
        var documentToWrite = AsyncApiDocumentFactory.CreateV3();
        using var stream = new MemoryStream();

        //act
        await this.DocumentWriter.WriteAsync(documentToWrite, stream, AsyncApiDocumentFormat.Yaml);
        await stream.FlushAsync();
        stream.Position = 0;
        var readDocument = await this.DocumentReader.ReadAsync(stream);

        //assert
        readDocument.Should().BeEquivalentTo(documentToWrite);
    }

    [Fact]
    public async Task Read_Yaml_Document_With_Unquoted_String_And_Enum_Scalars_Should_Work()
    {
        const string yaml = """
            asyncapi: 3.0.0
            info:
              title: Repro
              version: 1.0.0
            channels:
              hello:
                address: hello
            operations:
              greet:
                action: send
                channel:
                  $ref: '#/channels/hello'
                bindings:
                  mqtt:
                    qos: 2
            """;
        using var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(yaml));

        var document = await this.DocumentReader.ReadAsync(stream);

        var v3Document = document.Should().BeOfType<V3AsyncApiDocument>().Subject;
        v3Document.AsyncApi.Should().Be("3.0.0");
        v3Document.Info.Version.Should().Be("1.0.0");
        v3Document.Operations!["greet"].Bindings!.Mqtt!.QoS.Should().Be(MqttQualityOfServiceLevel.ExactlyOne);
    }

}
