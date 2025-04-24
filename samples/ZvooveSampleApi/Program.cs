// Copyright � 2021-Present Neuroglia SRL. All rights reserved.
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

using Json.Schema;
using Neuroglia.AsyncApi;
using Neuroglia.AsyncApi.Bindings.Amqp;
using Neuroglia.Data.Schemas.Json;
using ZvooveSampleApi;
using Json.Schema.Generation;
using Neuroglia.Serialization;
using System.Text.Json;
using JsonSerializer = Neuroglia.Serialization.Json.JsonSerializer;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient();
builder.Services.AddRazorPages()
    .AddRazorRuntimeCompilation();
builder.Services.AddAsyncApiUI();
builder.Services.AddAsyncApiGeneration(c =>
{
    c.Build();
});
builder.Services.AddAsyncApiDocument(document => document
    .UsingAsyncApiV3()
    .WithTitle("User Service API")
    .WithVersion("1.0.0")
    .WithDescription("The User Service with several usecases to try out the mapping of rabbitmq into asyncapi spec.")
    .WithLicense("Apache 2.0", new Uri("https://www.apache.org/licenses/LICENSE-2.0.html"))
    .WithDefaultContentType("application/json")
    .WithServer("production", server => server
        .WithHost("amqps://rabbit.zvoove-local.cloud")
        .WithProtocol(AsyncApiProtocol.Amqp)
        .WithBinding(new AmqpServerBindingDefinition()))
    .WithChannel("publicExchange", channel => channel
        // .WithServer("#/servers/production")
        .WithDescription("The endpoint used to publish and subscribe to user events")
        .WithBinding(new AmqpChannelBindingDefinition()
        {
            Type = AmqpChannelType.RoutingKey,
            Exchange = new AmqpExchangeDefinition()
            {
                Name = "users.events",
                Type = AmqpExchangeType.Topic,
                Durable = true,
                AutoDelete = false,
            },
        })
        .WithMessage("userCreated", message => message
            .Use("#/components/messages/userCreated"))
        .WithMessage("userUpdated", message => message
            .Use("#/components/messages/userUpdated"))
        .WithMessage("userDeleted", message => message
            .Use("#/components/messages/userDeleted")))
    .WithOperation("publicUserCreated", operation => operation
        .WithAction(Neuroglia.AsyncApi.v3.V3OperationAction.Send)
        .WithTitle("User created operation")
        .WithChannel("#/channels/publicExchange")
        .WithBinding(new AmqpOperationBindingDefinition()
        {
            Cc = [UserCreated.MessageType],
        })
        .WithMessage("#/channels/publicExchange/messages/userCreated"))
    .WithOperation("publicUserUpdated", operation => operation
        .WithAction(Neuroglia.AsyncApi.v3.V3OperationAction.Send)
        .WithTitle("User updated operation")
        .WithChannel("#/channels/publicExchange")
        .WithBinding(new AmqpOperationBindingDefinition()
        {
            Cc = [UserUpdated.MessageType],
        })
        .WithMessage("#/channels/publicExchange/messages/userUpdated"))
    .WithOperation("publicUserDeleted", operation => operation
        .WithAction(Neuroglia.AsyncApi.v3.V3OperationAction.Send)
        .WithTitle("User deleted operation")
        .WithChannel("#/channels/publicExchange")
        .WithBinding(new AmqpOperationBindingDefinition()
        {
            Cc = [UserDeleted.MessageType],
        })
        .WithMessage("#/channels/publicExchange/messages/userDeleted"))
    .WithMessageComponent("userCreated", message => message
        .WithName(UserCreated.MessageType)
        .WithDescription("User created event")
        .WithBinding(new AmqpMessageBindingDefinition()
        {
            MessageType = UserCreated.MessageType,
            ContentEncoding = "application/json",
        })
        .WithContentType("application/json")
        .WithPayloadSchema(schema => schema
            .WithJsonSchema(schemaBuilder => schemaBuilder.FromType<UserCreated>()))
        .WithCorrelationId(setup => setup
            .WithLocation("$message.payload#/subject")))
    .WithMessageComponent("userUpdated", message => message
        .WithName(UserUpdated.MessageType)
        .WithDescription("User created event")
        .WithBinding(new AmqpMessageBindingDefinition()
        {
            MessageType = UserUpdated.MessageType,
            ContentEncoding = "application/json",
        })
        .WithContentType("application/json")
        .WithPayloadSchema(schema => schema
            .WithJsonSchema(schemaBuilder => schemaBuilder.FromType<UserUpdated>()))
        .WithCorrelationId(setup => setup
            .WithLocation("$message.payload#/subject")))
    .WithMessageComponent("userDeleted", message => message
        .WithName(UserDeleted.MessageType)
        .WithDescription("User created event")
        .WithBinding(new AmqpMessageBindingDefinition()
        {
            MessageType = UserDeleted.MessageType,
            ContentEncoding = "application/json",
        })
        .WithContentType("application/json")
        .WithPayloadSchema(schema => schema
            .WithJsonSchema(schemaBuilder => schemaBuilder.FromType<UserDeleted>()))
        .WithCorrelationId(setup => setup
            .WithLocation("$message.payload#/subject"))));
builder.Services.AddSingleton<IJsonSchemaResolver, JsonSchemaResolver>();
builder.Services.AddSingleton<IJsonSerializer, JsonSerializer>();

var app = builder.Build();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.MapAsyncApiDocuments();
app.MapRazorPages();

app.Run();