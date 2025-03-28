﻿// Copyright © 2021-Present Neuroglia SRL. All rights reserved.
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

namespace Neuroglia.AsyncApi.Bindings.Amqp;

/// <summary>
/// Represents the object used to configure an AMQP 0.9+ message binding
/// </summary>
[DataContract]
public record AmqpMessageBindingDefinition
    : AmqpBindingDefinition, IMessageBindingDefinition
{

    /// <summary>
    /// Gets/sets a MIME encoding for the message content.
    /// </summary>
    [DataMember(Order = 1, Name = "contentEncoding"), JsonPropertyOrder(1), JsonPropertyName("contentEncoding"), YamlMember(Order = 1, Alias = "contentEncoding")]
    public virtual string? ContentEncoding { get; set; }

    /// <summary>
    /// Gets/sets a string that represents an application-specific message type.
    /// </summary>
    [DataMember(Order = 2, Name = "messageType"), JsonPropertyOrder(2), JsonPropertyName("messageType"), YamlMember(Order = 2, Alias = "messageType")]
    public virtual string? MessageType { get; set; }

    /// <summary>
    /// Gets/sets the version of this binding. Defaults to 'latest'.
    /// </summary>
    [DataMember(Order = 3, Name = "bindingVersion"), JsonPropertyOrder(3), JsonPropertyName("bindingVersion"), YamlMember(Order = 3, Alias = "bindingVersion")]
    public virtual string BindingVersion { get; set; } = "latest";

}
