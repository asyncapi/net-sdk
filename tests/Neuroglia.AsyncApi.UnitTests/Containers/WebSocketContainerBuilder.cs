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

using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;

namespace Neuroglia.AsyncApi.UnitTests.Containers;

public static class WebSocketContainerBuilder
{

    public const int PublicPort = 8080;

    public static IContainer Build()
    {
        return new ContainerBuilder()
            .WithName($"websocket-{Guid.NewGuid():N}")
            .WithImage("danpedigo/websocket-broadcast-server")
            .WithPortBinding(PublicPort, true)
            .WithWaitStrategy(Wait
                .ForUnixContainer()
                .UntilMessageIsLogged("HTTP server is listening on port: .*"))
            .Build();
    }

}
