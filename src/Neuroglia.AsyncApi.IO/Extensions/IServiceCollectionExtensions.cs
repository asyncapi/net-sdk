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

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Neuroglia.Serialization.Yaml;

namespace Neuroglia.AsyncApi.IO;

/// <summary>
/// Defines extensions for <see cref="IServiceCollection"/>s
/// </summary>
public static class IServiceCollectionExtensions
{

    /// <summary>
    /// Adds and configures Async API IO services
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to configure</param>
    /// <param name="lifetime">The lifetime of the services to add and configure</param>
    /// <returns>The configured <see cref="IServiceCollection"/></returns>
    public static IServiceCollection AddAsyncApiIO(this IServiceCollection services, ServiceLifetime lifetime = ServiceLifetime.Singleton)
    {
        services.AddSerialization();
        services.AddJsonSerializer();
        services.AddYamlDotNetSerializer(builder =>
        {
            YamlSerializer.DefaultSerializerConfiguration(builder.Serializer);
            YamlSerializer.DefaultDeserializerConfiguration(builder.Deserializer);
            builder.Deserializer
                .WithoutNodeTypeResolver<InferTypeResolver>()
                .WithNodeTypeResolver(new AsyncApiYamlTypeResolver());
        });
        services.TryAdd(new ServiceDescriptor(typeof(IAsyncApiDocumentReader), typeof(AsyncApiDocumentReader), lifetime: lifetime));
        services.TryAdd(new ServiceDescriptor(typeof(IAsyncApiDocumentWriter), typeof(AsyncApiDocumentWriter), lifetime: lifetime));
        return services;
    }

}
