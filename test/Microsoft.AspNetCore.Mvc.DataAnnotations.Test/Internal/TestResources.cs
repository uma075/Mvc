// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Globalization;

namespace Microsoft.AspNetCore.Mvc.ModelBinding
{
    // Wrap resources to make them available as public properties for [Display]. That attribute does not support
    // internal properties.
    public static class TestResources
    {
        public static string Type_Three_Name => "type three name " + CultureInfo.CurrentCulture;

        public static string DisplayAttribute_Description => DataAnnotations.Test.Resources.DisplayAttribute_Description;

        public static string DisplayAttribute_Name => DataAnnotations.Test.Resources.DisplayAttribute_Name;

        public static string DisplayAttribute_Prompt => DataAnnotations.Test.Resources.DisplayAttribute_Prompt;

        public static string DisplayAttribute_CultureSensitiveName =>
            DataAnnotations.Test.Resources.DisplayAttribute_Name + CultureInfo.CurrentUICulture;

        public static string DisplayAttribute_CultureSensitiveDescription =>
            DataAnnotations.Test.Resources.DisplayAttribute_Description + CultureInfo.CurrentUICulture;
    }
}