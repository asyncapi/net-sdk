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

using Neuroglia.AsyncApi.v2;

namespace Neuroglia.AsyncApi.IO;

/// <summary>
/// Defines the fundamentals of a service used to write <see cref="V2AsyncApiDocument"/>s
/// </summary>
public interface IAsyncApiDocumentWriter
{

    /// <summary>
    /// Writes the specified <see cref="V2AsyncApiDocument"/> to a <see cref="Stream"/>
    /// </summary>
    /// <param name="document">The <see cref="V2AsyncApiDocument"/> to write</param>
    /// <param name="stream">The <see cref="Stream"/> to read the <see cref="V2AsyncApiDocument"/> from</param>
    /// <param name="format">The format of the <see cref="V2AsyncApiDocument"/> to read. Defaults to '<see cref="AsyncApiDocumentFormat.Yaml"/>'</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new <see cref="V2AsyncApiDocument"/></returns>
    Task WriteAsync(V2AsyncApiDocument document, Stream stream, AsyncApiDocumentFormat format = AsyncApiDocumentFormat.Yaml, CancellationToken cancellationToken = default);

}
