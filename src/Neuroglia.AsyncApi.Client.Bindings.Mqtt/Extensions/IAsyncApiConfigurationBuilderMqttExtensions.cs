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

namespace Neuroglia.AsyncApi.Client.Bindings;

/// <summary>
/// Defines extensions for <see cref="IAsyncApiClientOptionsBuilder"/>s
/// </summary>
public static class IAsyncApiConfigurationBuilderMqttExtensions
{

    /// <summary>
    /// Adds and configures an <see cref="IBindingHandler"/> used to handle MQTT operations
    /// </summary>
    /// <param name="builder">The extended <see cref="IAsyncApiClientOptionsBuilder"/></param>
    /// <param name="setup">An <see cref="Action{T}"/>, if any, used to setup the <see cref="IBindingHandler"/>'s options</param>
    /// <returns>The configured <see cref="IAsyncApiClientOptionsBuilder"/></returns>
    public static IAsyncApiClientOptionsBuilder AddMqttBindingHandler(this IAsyncApiClientOptionsBuilder builder, Action<MqttBindingHandlerOptions>? setup = null)
    {
        setup ??= _ => { };
        builder.AddBindingHandler<MqttBindingHandler, MqttBindingHandlerOptions>(setup);
        return builder;
    }

}
