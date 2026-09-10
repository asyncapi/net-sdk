// Copyright © 2021-Present Neuroglia SRL. All rights reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using Neuroglia.Serialization.Yaml;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace Neuroglia.AsyncApi.IO;

internal sealed class AsyncApiYamlTypeResolver
    : INodeTypeResolver
{

    readonly InferTypeResolver inner = new();

    public bool Resolve(NodeEvent? nodeEvent, ref Type currentType)
    {
        if (currentType != typeof(object)) return false;
        return this.inner.Resolve(nodeEvent, ref currentType);
    }

}
