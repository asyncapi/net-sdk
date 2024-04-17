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

using Neuroglia.Serialization.Json.Converters;
using System.ComponentModel;

namespace Neuroglia.AsyncApi.v2.Bindings.Sns;

/// <summary>
/// Enumerates all supported SNS topic ordering types
/// </summary>
[JsonConverter(typeof(StringEnumConverter))]
[TypeConverter(typeof(EnumMemberTypeConverter))]
public enum SnsTopicOrderingType
{
    /// <summary>
    /// Indicates a standard SNS topic
    /// </summary>
    [EnumMember(Value = "standard")]
    Standard,
    /// <summary>
    /// Indicates a FIFO SNS topic
    /// </summary>
    [EnumMember(Value = "FIFO")]
    Fifo
}