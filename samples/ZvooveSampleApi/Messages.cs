#region License
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
#endregion

using Neuroglia.AsyncApi.v3;

namespace ZvooveSampleApi;


[Message(Name = MessageType, Description = "Internal event for keeping user data in sync on update")]
public record InternalUserEvent
{
    public const string MessageType = "internal.user.event";
    
    public int UserId { get; init; }
}
//
// [MessageIdentity(MessageType)]
// [AsyncApiMessage(MessageType, Summary = "User was created.")]
// [RabbitMqMessageBinding(MessageType = MessageType)]
public record UserCreated
{
    public const string MessageType = "user.created";
    
    public int UserId { get; init; }
}
//
// [MessageIdentity(MessageType)]
// [AsyncApiMessage(MessageType, Summary = "User was updated.")]
// [RabbitMqMessageBinding(MessageType = MessageType)]
public record UserUpdated
{
    public const string MessageType = "user.updated";

    public int UserId { get; init; }
}
//
// [MessageIdentity(MessageType)]
// [AsyncApiMessage(MessageType, Summary = "User was deleted.")]
// [RabbitMqMessageBinding(MessageType = MessageType)]
public record UserDeleted
{
    public const string MessageType = "user.deleted";

    public int UserId { get; init; }
}
//
// [MessageIdentity(MessageType)]
// [AsyncApiMessage(MessageType, Summary = "User is requested to be disabled.")]
// [RabbitMqMessageBinding(MessageType = MessageType)]
// public record DisableUser
// {
//     public const string MessageType = "user.disable";
//
//     public int UserId { get; init; }
// }
//
// [MessageIdentity(MessageType)]
// [AsyncApiMessage(MessageType, Summary = "User authorization rules changed.")]
// [RabbitMqMessageBinding(MessageType = MessageType)]
// public record AuthzChanged
// {
//     public const string MessageType = "authz.changed";
//
//     public int UserId { get; init; }
// }