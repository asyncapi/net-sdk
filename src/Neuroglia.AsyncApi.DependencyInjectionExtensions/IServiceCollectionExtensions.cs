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

using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Neuroglia.AsyncApi.FluentBuilders;
using Neuroglia.AsyncApi.FluentBuilders.v2;
using Neuroglia.AsyncApi.FluentBuilders.v3;
using Neuroglia.AsyncApi.Generation;
using Neuroglia.AsyncApi.IO;
using Neuroglia.AsyncApi.Validation.v2;
using Neuroglia.Serialization;

namespace Neuroglia.AsyncApi;

/// <summary>
/// Defines extensions for <see cref="IServiceCollection"/>s
/// </summary>
public static class IServiceCollectionExtensions
{

    /// <summary>
    /// Adds and configures Async API services (<see cref="ISerializer"/>s, <see cref="IAsyncApiDocumentReader"/>, <see cref="IAsyncApiDocumentWriter"/>, ...)
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to configure</param>
    /// <returns>The configured <see cref="IServiceCollection"/></returns>
    public static IServiceCollection AddAsyncApi(this IServiceCollection services)
    {
        services.AddAsyncApiIO();
        services.TryAddSingleton<IAsyncApiDocumentReader, AsyncApiDocumentReader>();
        services.TryAddSingleton<IAsyncApiDocumentWriter, AsyncApiDocumentWriter>();
        services.TryAddTransient<IAsyncApiDocumentBuilder, AsyncApiDocumentBuilder>();
        services.TryAddTransient<IV2AsyncApiDocumentBuilder, V2AsyncApiDocumentBuilder>();
        services.TryAddTransient<IV3AsyncApiDocumentBuilder, V3AsyncApiDocumentBuilder>();
        services.AddValidatorsFromAssemblyContaining<V2AsyncApiDocumentValidator>(ServiceLifetime.Transient);
        return services;
    }

    /// <summary>
    /// Adds and configures code-first AsyncAPI document generation
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to configure</param>
    /// <param name="setup">An <see cref="Action{T}"/> used to configure the <see cref="AsyncApiDocumentGenerationOptions"/> to use</param>
    /// <returns>The configured <see cref="IServiceCollection"/></returns>
    public static IServiceCollection AddAsyncApiGeneration(this IServiceCollection services, Action<IAsyncApiGenerationOptionsBuilder> setup)
    {
        services.AddAsyncApi();
        services.Configure<AsyncApiGenerationOptions>(options =>
        {
            var builder = new AsyncApiGenerationOptionsBuilder(options);
            setup(builder);
        });
        services.TryAddTransient<IJsonSchemaExampleGenerator, JsonSchemaExampleGenerator>();
        services.TryAddTransient<IAsyncApiDocumentGenerator, AsyncApiDocumentGenerator>();
        services.TryAddSingleton<AsyncApiDocumentProvider>();
        services.TryAddSingleton<IAsyncApiDocumentProvider>(provider => provider.GetRequiredService<AsyncApiDocumentProvider>());
        services.AddSingleton<IHostedService>(provider => provider.GetRequiredService<AsyncApiDocumentProvider>());
        return services;
    }

    /// <summary>
    /// Adds and configures a new <see cref="IAsyncApiDocument"/>
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to configure</param>
    /// <param name="document">The <see cref="IAsyncApiDocument"/> to add</param>
    /// <returns>The configured <see cref="IServiceCollection"/></returns>
    public static IServiceCollection AddAsyncApiDocument(this IServiceCollection services, IAsyncApiDocument document)
    {
        services.AddSingleton(document);
        return services;
    }

    /// <summary>
    /// Adds and configures a new <see cref="IAsyncApiDocument"/>
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to configure</param>
    /// <param name="setup">An <see cref="Action{T}"/> used to setup the <see cref="IV2AsyncApiDocumentBuilder"/> used to build the <see cref="IAsyncApiDocument"/> to add</param>
    /// <returns>The configured <see cref="IServiceCollection"/></returns>
    public static IServiceCollection AddAsyncApiDocument(this IServiceCollection services, Action<IServiceProvider, IAsyncApiDocumentBuilder> setup)
    {
        services.AddSingleton(provider =>
        {
            var builder = provider.GetRequiredService<IAsyncApiDocumentBuilder>();
            setup(provider, builder);
            return builder.Build();
        });
        return services;
    }

    /// <summary>
    /// Adds and configures a new <see cref="IAsyncApiDocument"/>
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to configure</param>
    /// <param name="setup">An <see cref="Action{T}"/> used to setup the <see cref="IV2AsyncApiDocumentBuilder"/> used to build the <see cref="IAsyncApiDocument"/> to add</param>
    /// <returns>The configured <see cref="IServiceCollection"/></returns>
    public static IServiceCollection AddAsyncApiDocument(this IServiceCollection services, Action<IAsyncApiDocumentBuilder> setup)
    {
        services.AddSingleton(provider =>
        {
            var builder = provider.GetRequiredService<IAsyncApiDocumentBuilder>();
            setup(builder);
            return builder.Build();
        });
        return services;
    }

}
