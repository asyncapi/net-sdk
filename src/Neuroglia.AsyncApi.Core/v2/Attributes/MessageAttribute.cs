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

namespace Neuroglia.AsyncApi.v2;

/// <summary>
/// Represents an <see cref="Attribute"/> used to configure an <see cref="V2OperationDefinition"/>'s <see cref="V2MessageDefinition"/>
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
public class MessageAttribute
    : Attribute
{

    /// <summary>
    /// Gets/sets the <see cref="V2MessageDefinition"/>'s name
    /// </summary>
    public virtual string? Name { get; set; }

    /// <summary>
    /// Gets/sets the <see cref="V2MessageDefinition"/>'s title
    /// </summary>
    public virtual string? Title { get; set; }

    /// <summary>
    /// Gets/sets the <see cref="V2MessageDefinition"/>'s description
    /// </summary>
    public virtual string? Description { get; set; }

    /// <summary>
    /// Gets/sets the <see cref="V2MessageDefinition"/>'s summary
    /// </summary>
    public virtual string? Summary { get; set; }

    /// <summary>
    /// Gets/sets the <see cref="V2MessageDefinition"/>'s content type
    /// </summary>
    public virtual string? ContentType { get; set; }

}
