// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Internal;

namespace Microsoft.AspNetCore.Mvc.Controllers
{
    /// <summary>
    /// Default implementation for <see cref="IControllerFactory"/>.
    /// </summary>
    public class DefaultControllerFactory : IControllerFactory
    {
        private readonly IControllerActivator _controllerActivator;
        private readonly IControllerPropertyActivator[] _propertyActivators;

        /// <summary>
        /// Initializes a new instance of <see cref="DefaultControllerFactory"/>.
        /// </summary>
        /// <param name="controllerActivator">
        /// <see cref="IControllerActivator"/> used to create controller instances.
        /// </param>
        /// <param name="propertyActivators">
        /// A set of <see cref="IControllerPropertyActivator"/> instances used to initialize controller
        /// properties.
        /// </param>
        public DefaultControllerFactory(
            IControllerActivator controllerActivator,
#pragma warning disable PUB0001 // Pubternal type in public API
            IEnumerable<IControllerPropertyActivator> propertyActivators
#pragma warning restore PUB0001
            )
        {
            if (controllerActivator == null)
            {
                throw new ArgumentNullException(nameof(controllerActivator));
            }

            if (propertyActivators == null)
            {
                throw new ArgumentNullException(nameof(propertyActivators));
            }

            _controllerActivator = controllerActivator;
            _propertyActivators = propertyActivators.ToArray();
        }

        /// <summary>
        /// The <see cref="IControllerActivator"/> used to create a controller.
        /// </summary>
        protected IControllerActivator ControllerActivator => _controllerActivator;

        /// <inheritdoc />
        public virtual object CreateController(ControllerContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (context.ActionDescriptor == null)
            {
                throw new ArgumentException(Resources.FormatPropertyOfTypeCannotBeNull(
                    nameof(ControllerContext.ActionDescriptor),
                    nameof(ControllerContext)));
            }

            var controller = _controllerActivator.Create(context);
            foreach (var propertyActivator in _propertyActivators)
            {
                propertyActivator.Activate(context, controller);
            }

            return controller;
        }

        /// <inheritdoc />
        public virtual void ReleaseController(ControllerContext context, object controller)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (controller == null)
            {
                throw new ArgumentNullException(nameof(controller));
            }

            _controllerActivator.Release(context, controller);
        }
    }
}
