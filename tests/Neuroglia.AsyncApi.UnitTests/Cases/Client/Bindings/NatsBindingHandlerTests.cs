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

using Microsoft.Extensions.Primitives;
using NATS.Client.Core;
using NATS.Net;
using Neuroglia.AsyncApi.Bindings.Nats;
using Neuroglia.AsyncApi.Client;
using Neuroglia.AsyncApi.Client.Bindings;
using Neuroglia.AsyncApi.UnitTests.Containers;

namespace Neuroglia.AsyncApi.UnitTests.Cases.Client.Bindings;

public class NatsBindingHandlerTests
    : BindingHandlerTestsBase
{
    public NatsBindingHandlerTests()
        : base(builder => builder.AddNatsBindingHandler(), ConfigureServices)
    {

    }

    [Fact]
    public async Task Publish_Should_Work()
    {
        //arrange
        var serverId = "nats-server";
        var channelId = "cloud-events";
        var operationId = "publishCloudEvent";
        var messageId = "cloudEvent";
        var stringSchema = new JsonSchemaBuilder().Type(SchemaValueType.String).Build();
        var objectSchema = new JsonSchemaBuilder().Type(SchemaValueType.Object).AdditionalProperties(true).Build();
        var document = DocumentBuilder
            .UsingAsyncApiV3()
            .WithTitle("Test Nats API")
            .WithVersion("1.0.0")
            .WithServer(serverId, server => server
                .WithHost($"localhost:{ServiceProvider.GetRequiredKeyedService<DotNet.Testcontainers.Containers.IContainer>("nats").GetMappedPublicPort(NatsContainerBuilder.PublicPort)}")
                .WithProtocol(AsyncApiProtocol.Nats)
                .WithBinding(new NatsServerBindingDefinition()))
            .WithChannel(channelId, channel => channel
                .WithAddress("cloud-event")
                .WithServer($"#/servers/{serverId}")
                .WithMessage(messageId, message => message
                    .WithContentType(CloudEventContentType.Json)
                    .WithPayloadSchema(schemaDefinition => schemaDefinition
                        .WithJsonSchema(schema => schema
                            .Type(SchemaValueType.Object)
                            .Properties(new Dictionary<string, JsonSchema>()
                            {
                                { CloudEventAttributes.SpecVersion, stringSchema },
                                { CloudEventAttributes.Id, stringSchema },
                                { CloudEventAttributes.Time, stringSchema },
                                { CloudEventAttributes.Source, stringSchema },
                                { CloudEventAttributes.Type, stringSchema },
                                { CloudEventAttributes.Subject, stringSchema },
                                { CloudEventAttributes.DataSchema, stringSchema },
                                { CloudEventAttributes.DataContentType, stringSchema },
                                { CloudEventAttributes.Data, objectSchema },
                            })
                            .Required(CloudEventAttributes.GetRequiredAttributes())
                            .AdditionalProperties(true)))
                    .WithBinding(new NatsMessageBindingDefinition()))
                .WithBinding(new NatsChannelBindingDefinition()))
            .WithOperation(operationId, operation => operation
                .WithAction(v3.V3OperationAction.Receive)
                .WithChannel($"#/channels/{channelId}")
                .WithMessage($"#/channels/{channelId}/messages/{messageId}")
                .WithBinding(new NatsOperationBindingDefinition()))
            .Build();
        await using var client = ClientFactory.CreateFor(document);

        //act
        var e = new CloudEvent()
        {
            Id = Guid.NewGuid().ToString(),
            SpecVersion = CloudEventSpecVersion.V1.Version,
            Source = new("https://unit-tests.v3.asyncapi.neuroglia.io"),
            Type = "io.neuroglia.asyncapi.v3.test.v1",
            DataContentType = MediaTypeNames.Application.Json,
            Data = new
            {
                Greetings = "Hello, World!"
            }
        };
        var parameters = new AsyncApiPublishOperationParameters(operationId)
        {
            Payload = e
        };
        await using var result = await client.PublishAsync(parameters);

        //assert
        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public async Task Subscribe_Should_Work()
    {
        //arrange
        var serverId = "nats-server";
        var serverAddress = $"localhost:{ServiceProvider.GetRequiredKeyedService<DotNet.Testcontainers.Containers.IContainer>("nats").GetMappedPublicPort(NatsContainerBuilder.PublicPort)}";
        var channelId = "cloud-events";
        var channelAddress = "cloud-event";
        var operationId = "subscribeToCloudEvents";
        var messageId = "cloud-event";
        var stringSchema = new JsonSchemaBuilder().Type(SchemaValueType.String).Build();
        var objectSchema = new JsonSchemaBuilder().Type(SchemaValueType.Object).AdditionalProperties(true).Build();
        var document = DocumentBuilder
            .UsingAsyncApiV3()
            .WithTitle("Test Nats API")
            .WithVersion("1.0.0")
            .WithServer(serverId, server => server
                .WithHost(serverAddress)
                .WithProtocol(AsyncApiProtocol.Nats)
                .WithBinding(new NatsServerBindingDefinition()))
            .WithChannel(channelId, channel => channel
                .WithAddress(channelAddress)
                .WithServer($"#/servers/{serverId}")
                .WithMessage(messageId, message => message
                    .WithContentType(CloudEventContentType.Json)
                    .WithPayloadSchema(schemaDefinition => schemaDefinition
                        .WithJsonSchema(schema => schema
                            .Type(SchemaValueType.Object)
                            .Properties(new Dictionary<string, JsonSchema>()
                            {
                                { CloudEventAttributes.SpecVersion, stringSchema },
                                { CloudEventAttributes.Id, stringSchema },
                                { CloudEventAttributes.Time, stringSchema },
                                { CloudEventAttributes.Source, stringSchema },
                                { CloudEventAttributes.Type, stringSchema },
                                { CloudEventAttributes.Subject, stringSchema },
                                { CloudEventAttributes.DataSchema, stringSchema },
                                { CloudEventAttributes.DataContentType, stringSchema },
                                { CloudEventAttributes.Data, objectSchema },
                            })
                            .Required(CloudEventAttributes.GetRequiredAttributes())
                            .AdditionalProperties(true)))
                    .WithBinding(new NatsMessageBindingDefinition()))
                .WithBinding(new NatsChannelBindingDefinition()))
            .WithOperation(operationId, operation => operation
                .WithAction(v3.V3OperationAction.Send)
                .WithChannel($"#/channels/{channelId}")
                .WithMessage($"#/channels/{channelId}/messages/{messageId}")
                .WithBinding(new NatsOperationBindingDefinition()))
            .Build();
        await using var client = ClientFactory.CreateFor(document);

        //act
        var parameters = new AsyncApiSubscribeOperationParameters(operationId);
        await using var result = await client.SubscribeAsync(parameters);
        var messageCount = 10;
        var messagesToSend = new List<Tuple<byte[], NatsHeaders>>();
        for (var i = 0; i < messageCount; i++)
        {
            var e = new CloudEvent()
            {
                Id = Guid.NewGuid().ToString(),
                SpecVersion = CloudEventSpecVersion.V1.Version,
                Source = new("https://unit-tests.v3.asyncapi.neuroglia.io"),
                Subject = i.ToString(),
                Type = "io.neuroglia.asyncapi.v3.test.v1",
                DataContentType = MediaTypeNames.Application.Json,
                Data = new
                {
                    Greetings = "Hello, World!"
                }
            };
            var payload = JsonSerializer.Default.SerializeToByteArray(e)!;
            var headers = new NatsHeaders
            {
                { "Header1", new StringValues("value1") },
                { "Header2", new StringValues("value2") },
                { "Header3", new StringValues("value3") }
            };
            messagesToSend.Add(new(payload, headers));
        }
        var messagesReceived = new List<IAsyncApiMessage>();
        var subscription = result.Messages?.Subscribe(messagesReceived.Add);
        await using var nats = new NatsClient(serverAddress);
        // NATS.Net establishes async-enumerable subscriptions in the background.
        await Task.Delay(1000);
        foreach (var message in messagesToSend) await nats.PublishAsync(channelAddress, message.Item1, message.Item2);
        await Task.Delay(3500);
        subscription?.Dispose();

        //assert
        result.IsSuccessful.Should().BeTrue();
        result.Messages.Should().NotBeNull();
        messagesReceived.Should().NotBeEmpty();
    }

    static void ConfigureServices(IServiceCollection services)
    {
        services.AddKeyedSingleton("nats", NatsContainerBuilder.Build());
        services.AddSingleton(provider => provider.GetRequiredKeyedService<DotNet.Testcontainers.Containers.IContainer>("nats"));
        services.AddHostedService<ContainerBootstrapper>();
    }

}
