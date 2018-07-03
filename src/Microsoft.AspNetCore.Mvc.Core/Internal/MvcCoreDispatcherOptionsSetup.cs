// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;

namespace Microsoft.AspNetCore.Mvc.Internal
{
    /// <summary>
    /// Sets up MVC default options for <see cref="DispatcherOptions"/>.
    /// </summary>
    public class MvcCoreDispatcherOptionsSetup : IConfigureOptions<DispatcherOptions>
    {
        /// <summary>
        /// Configures the <see cref="DispatcherOptions"/>.
        /// </summary>
        /// <param name="options">The <see cref="DispatcherOptions"/>.</param>
        public void Configure(DispatcherOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            options.ConstraintMap.Add("exists", typeof(KnownRouteValueEndpointMatchConstraint));
        }
    }
}
