// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Core;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Routing;

namespace Microsoft.AspNetCore.Mvc.Routing
{
    public class KnownRouteValueEndpointMatchConstraint : IEndpointMatchConstraint
    {
        private RouteValuesCollection _cachedValuesCollection;
        private readonly IActionDescriptorCollectionProvider _actionDescriptorCollectionProvider;

        public KnownRouteValueEndpointMatchConstraint(
            IActionDescriptorCollectionProvider actionDescriptorCollectionProvider)
        {
            _actionDescriptorCollectionProvider = actionDescriptorCollectionProvider;
        }

        public bool Match(
            string routeKey,
            RouteValueDictionary values,
            RouteDirection routeDirection)
        {
            if (routeKey == null)
            {
                throw new ArgumentNullException(nameof(routeKey));
            }

            if (values == null)
            {
                throw new ArgumentNullException(nameof(values));
            }

            object obj;
            if (values.TryGetValue(routeKey, out obj))
            {
                var value = obj as string;
                if (value != null)
                {
                    var allValues = GetAndCacheAllMatchingValues(routeKey);
                    foreach (var existingValue in allValues)
                    {
                        if (string.Equals(value, existingValue, StringComparison.OrdinalIgnoreCase))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        private string[] GetAndCacheAllMatchingValues(string routeKey)
        {
            var actionDescriptors = GetAndValidateActionDescriptorCollection();
            var version = actionDescriptors.Version;
            var valuesCollection = _cachedValuesCollection;

            if (valuesCollection == null ||
                version != valuesCollection.Version)
            {
                var values = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                for (var i = 0; i < actionDescriptors.Items.Count; i++)
                {
                    var action = actionDescriptors.Items[i];

                    string value;
                    if (action.RouteValues.TryGetValue(routeKey, out value) &&
                        !string.IsNullOrEmpty(value))
                    {
                        values.Add(value);
                    }
                }

                valuesCollection = new RouteValuesCollection(version, values.ToArray());
                _cachedValuesCollection = valuesCollection;
            }

            return _cachedValuesCollection.Items;
        }

        private ActionDescriptorCollection GetAndValidateActionDescriptorCollection()
        {
            var descriptors = _actionDescriptorCollectionProvider.ActionDescriptors;

            if (descriptors == null)
            {
                throw new InvalidOperationException(
                    Resources.FormatPropertyOfTypeCannotBeNull("ActionDescriptors",
                                                               _actionDescriptorCollectionProvider.GetType()));
            }

            return descriptors;
        }

        public void Initialize(string parameter)
        {
        }

        private class RouteValuesCollection
        {
            public RouteValuesCollection(int version, string[] items)
            {
                Version = version;
                Items = items;
            }

            public int Version { get; }

            public string[] Items { get; }
        }
    }
}
