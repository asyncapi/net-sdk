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

namespace Neuroglia.AsyncApi.FluentBuilders;

/// <summary>
/// Defines the fundamentals of a service used to build <see cref="ExternalDocumentationDefinition"/>s
/// </summary>
public interface IExternalDocumentationDefinitionBuilder
    : IReferenceableComponentDefinitionBuilder<ExternalDocumentationDefinition>
{

    /// <summary>
    ///  Configures the <see cref="ExternalDocumentationDefinition"/> to build to use the specified url
    /// </summary>
    /// <param name="url">The url of the <see cref="ExternalDocumentationDefinition"/> to build</param>
    /// <returns>The configured <see cref="IExternalDocumentationDefinitionBuilder"/></returns>
    IExternalDocumentationDefinitionBuilder WithUrl(Uri url);

    /// <summary>
    ///  Configures the <see cref="ExternalDocumentationDefinition"/> to build to use the specified description
    /// </summary>
    /// <param name="description">The description of the <see cref="ExternalDocumentationDefinition"/> to build</param>
    /// <returns>The configured <see cref="IExternalDocumentationDefinitionBuilder"/></returns>
    IExternalDocumentationDefinitionBuilder WithDescription(string? description);

    /// <summary>
    /// Builds the configured <see cref="ExternalDocumentationDefinition"/>
    /// </summary>
    /// <returns>A new <see cref="ExternalDocumentationDefinition"/></returns>
    ExternalDocumentationDefinition Build();

}
