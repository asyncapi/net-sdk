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

namespace Neuroglia.AsyncApi.Validation.v3;

/// <summary>
/// Represents the service used to validate the <see cref="V3ApiInfo"/>
/// </summary>
public class V3ApiInfoValidator
    : AbstractValidator<V3ApiInfo>
{

    /// <inheritdoc/>
    public V3ApiInfoValidator(V3AsyncApiDocument? document = null)
    {
        this.RuleFor(i => i.Title)
            .NotEmpty();
        this.RuleFor(i => i.Version)
            .NotEmpty();
        this.RuleFor(i => i.License!)
            .SetValidator(new V3LicenseValidator());
        this.RuleFor(i => i.ExternalDocs!)
            .SetValidator(new V3ExternalDocumentationValidator(document));
        this.RuleForEach(i => i.Tags)
            .SetValidator(new V3TagValidator(document));
    }

}
