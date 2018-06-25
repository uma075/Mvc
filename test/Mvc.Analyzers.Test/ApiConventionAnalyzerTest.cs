// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Analyzer.Testing;
using Microsoft.AspNetCore.Mvc.Analyzers.Infrastructure;
using Microsoft.CodeAnalysis;
using Xunit;
using static Microsoft.AspNetCore.Mvc.Analyzers.ApiConventionAnalyzer;

namespace Microsoft.AspNetCore.Mvc.Analyzers
{
    public class ApiConventionAnalyzerTest
    {
        [Fact]
        public Task UnwrapMethodReturnType_ReturnsType_ForSimpleTypes()
        {
            // Arrange
            var expectedTypeName = typeof(ApiConventionAnalyzerBaseModel).FullName;
            var methodName = nameof(UnwrapMethodReturnType.ReturnsBaseModel);

            return AssertUnwrapMethodReturnType(expectedTypeName, methodName);
        }

        [Fact]
        public Task UnwrapMethodReturnType_ReturnsType_ForActionResultOfT()
        {
            // Arrange
            var expectedTypeName = typeof(ApiConventionAnalyzerBaseModel).FullName;
            var methodName = nameof(UnwrapMethodReturnType.ReturnsActionResultOfBaseModel);

            return AssertUnwrapMethodReturnType(expectedTypeName, methodName);
        }

        [Fact]
        public Task UnwrapMethodReturnType_ReturnsType_ForTaskOfActionResultOfT()
        {
            // Arrange
            var expectedTypeName = typeof(ApiConventionAnalyzerBaseModel).FullName;
            var methodName = nameof(UnwrapMethodReturnType.ReturnsTaskOfActionResultOfBaseModel);

            return AssertUnwrapMethodReturnType(expectedTypeName, methodName);
        }

        [Fact]
        public Task UnwrapMethodReturnType_ReturnsType_ForValueTaskOfActionResultOfT()
        {
            // Arrange
            var expectedTypeName = typeof(ApiConventionAnalyzerBaseModel).FullName;
            var methodName = nameof(UnwrapMethodReturnType.ReturnsValueTaskOfActionResultOfBaseModel);

            return AssertUnwrapMethodReturnType(expectedTypeName, methodName);
        }

        [Fact]
        public async Task UnwrapMethodReturnType_ReturnsType_ForIEnumerableOfT()
        {
            // Arrange
            var methodName = nameof(UnwrapMethodReturnType.ReturnsActionResultOfIEnumerableOfBaseModel);
            var compilation = await GetCompilation();
            var symbolCache = new ApiControllerSymbolCache(compilation);

            var ienumerable = compilation.GetTypeByMetadataName(typeof(IEnumerable<>).FullName);
            var baseModel = compilation.GetTypeByMetadataName(typeof(ApiConventionAnalyzerBaseModel).FullName);
            var expected = ienumerable.Construct(baseModel);
            var type = compilation.GetTypeByMetadataName(typeof(UnwrapMethodReturnType).FullName);
            var method = (IMethodSymbol)type.GetMembers(methodName).First();
            var returnType = method.ReturnType;
            var syntaxTree = compilation.GetSemanticModel(type.DeclaringSyntaxReferences[0].SyntaxTree);

            // Act
            var actual = ApiConventionAnalyzer.UnwrapMethodReturnType(symbolCache, syntaxTree, method);

            // Assert
            Assert.Equal(expected, actual);
        }

        private async Task AssertUnwrapMethodReturnType(string expectedTypeName, string methodName)
        {
            var compilation = await GetCompilation();
            var symbolCache = new ApiControllerSymbolCache(compilation);

            var expected = compilation.GetTypeByMetadataName(expectedTypeName);
            var type = compilation.GetTypeByMetadataName(typeof(UnwrapMethodReturnType).FullName);
            var method = (IMethodSymbol)type.GetMembers(methodName).First();
            var returnType = method.ReturnType;
            var syntaxTree = compilation.GetSemanticModel(type.DeclaringSyntaxReferences[0].SyntaxTree);

            // Act
            var actual = ApiConventionAnalyzer.UnwrapMethodReturnType(symbolCache, syntaxTree, method);

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task GetDefaultStatusCode_ReturnsValueDefinedUsingStatusCodeConstants()
        {
            // Arrange
            var compilation = await GetCompilation();
            var attribute = compilation.GetTypeByMetadataName(typeof(TestActionResultUsingStatusCodesConstants).FullName).GetAttributes()[0];

            // Act
            var actual = ApiConventionAnalyzer.GetDefaultStatusCode(attribute);

            // Assert
            Assert.Equal(412, actual);
        }

        [Fact]
        public async Task GetDefaultStatusCode_ReturnsValueDefinedUsingHttpStatusCast()
        {
            // Arrange
            var compilation = await GetCompilation();
            var attribute = compilation.GetTypeByMetadataName(typeof(TestActionResultUsingHttpStatusCodeCast).FullName).GetAttributes()[0];

            // Act
            var actual = ApiConventionAnalyzer.GetDefaultStatusCode(attribute);

            // Assert
            Assert.Equal(103, actual);
        }

        [Fact]
        public async Task InspectReturnExpression_ReturnsNull_ForReturnTypeIf200StatusCodeIsDeclared()
        {
            // Arrange
            var compilation = await GetCompilation();

            var returnType = compilation.GetTypeByMetadataName(typeof(ApiConventionAnalyzerBaseModel).FullName);
            var context = GetContext(compilation, returnType, new[] { 200 });

            // Act
            var diagnostic = ApiConventionAnalyzer.InspectReturnExpression(context, returnType, Location.None);

            // Assert
            Assert.Null(diagnostic);
        }

        [Fact]
        public async Task InspectReturnExpression_ReturnsNull_ForReturnTypeIf201StatusCodeIsDeclared()
        {
            // Arrange
            var compilation = await GetCompilation();

            var returnType = compilation.GetTypeByMetadataName(typeof(ApiConventionAnalyzerBaseModel).FullName);
            var context = GetContext(compilation, returnType, new[] { 201 });

            // Act
            var diagnostic = ApiConventionAnalyzer.InspectReturnExpression(context, returnType, Location.None);

            // Assert
            Assert.Null(diagnostic);
        }

        [Fact]
        public async Task InspectReturnExpression_ReturnsNull_ForDerivedReturnTypeIf200StatusCodeIsDeclared()
        {
            // Arrange
            var compilation = await GetCompilation();

            var declaredReturnType = compilation.GetTypeByMetadataName(typeof(ApiConventionAnalyzerBaseModel).FullName);
            var context = GetContext(compilation, declaredReturnType, new[] { 201 });
            var actualReturnType = compilation.GetTypeByMetadataName(typeof(ApiConventionAnalyzerDerivedModel).FullName);

            // Act
            var diagnostic = ApiConventionAnalyzer.InspectReturnExpression(context, actualReturnType, Location.None);

            // Assert
            Assert.Null(diagnostic);
        }

        [Fact]
        public async Task InspectReturnExpression_ReturnsDiagnostic_If200IsNotDocumented()
        {
            // Arrange
            var compilation = await GetCompilation();

            var declaredReturnType = compilation.GetTypeByMetadataName(typeof(ApiConventionAnalyzerBaseModel).FullName);
            var context = GetContext(compilation, declaredReturnType, new[] { 404 });
            var actualReturnType = compilation.GetTypeByMetadataName(typeof(ApiConventionAnalyzerDerivedModel).FullName);

            // Act
            var diagnostic = ApiConventionAnalyzer.InspectReturnExpression(context, actualReturnType, Location.None);

            // Assert
            Assert.NotNull(diagnostic);
            Assert.Same(DiagnosticDescriptors.MVC1005_ActionReturnsUndocumentedSuccessResult, diagnostic.Descriptor);
        }

        [Fact]
        public async Task InspectReturnExpression_ReturnsDiagnostic_IfReturnTypeIsActionResultReturningUndocumentedStatusCode()
        {
            // Arrange
            var compilation = await GetCompilation();

            var declaredReturnType = compilation.GetTypeByMetadataName(typeof(ApiConventionAnalyzerBaseModel).FullName);
            var context = GetContext(compilation, declaredReturnType, new[] { 200, 404 });
            var actualReturnType = compilation.GetTypeByMetadataName(typeof(BadRequestObjectResult).FullName);

            // Act
            var diagnostic = ApiConventionAnalyzer.InspectReturnExpression(context, actualReturnType, Location.None);

            // Assert
            Assert.NotNull(diagnostic);
            Assert.Same(DiagnosticDescriptors.MVC1004_ActionReturnsUndocumentedStatusCode, diagnostic.Descriptor);
        }

        [Fact]
        public async Task InspectReturnExpression_DoesNotReturnDiagnostic_IfReturnTypeDoesNotHaveStatusCodeAttribute()
        {
            // Arrange
            var compilation = await GetCompilation();

            var declaredReturnType = compilation.GetTypeByMetadataName(typeof(ApiConventionAnalyzerBaseModel).FullName);
            var context = GetContext(compilation, declaredReturnType, new[] { 200, 404 });
            var actualReturnType = compilation.GetTypeByMetadataName(typeof(EmptyResult).FullName);

            // Act
            var diagnostic = ApiConventionAnalyzer.InspectReturnExpression(context, actualReturnType, Location.None);

            // Assert
            Assert.Null(diagnostic);
        }

        [Fact]
        public async Task InspectReturnExpression_DoesNotReturnDiagnostic_IfDeclaredAndActualReturnTypeAreIActionResultInstances()
        {
            // Arrange
            var compilation = await GetCompilation();

            var declaredReturnType = compilation.GetTypeByMetadataName(typeof(IActionResult).FullName);
            var context = GetContext(compilation, declaredReturnType, new[] { 404 });
            var actualReturnType = compilation.GetTypeByMetadataName(typeof(EmptyResult).FullName);

            // Act
            var diagnostic = ApiConventionAnalyzer.InspectReturnExpression(context, actualReturnType, Location.None);

            // Assert
            Assert.Null(diagnostic);
        }

        [Fact]
        public async Task InspectReturnExpression_DoesNotReturnDiagnostic_IfDeclaredAndActualReturnTypeAreIActionResult()
        {
            // Arrange
            var compilation = await GetCompilation();

            var declaredReturnType = compilation.GetTypeByMetadataName(typeof(IActionResult).FullName);
            var context = GetContext(compilation, declaredReturnType, new[] { 404 });
            var actualReturnType = compilation.GetTypeByMetadataName(typeof(IActionResult).FullName);

            // Act
            var diagnostic = ApiConventionAnalyzer.InspectReturnExpression(context, actualReturnType, Location.None);

            // Assert
            Assert.Null(diagnostic);
        }

        private static ApiConventionContext GetContext(Compilation compilation, INamedTypeSymbol returnType, int[] expectedStatusCodes)
        {
            var symbolCache = new ApiControllerSymbolCache(compilation);
            var context = new ApiConventionContext(
                symbolCache,
                default,
                expectedStatusCodes.Select(s => new ApiResponseMetadata(s, null, null)).ToArray(),
                new HashSet<int>(),
                returnType);
            return context;
        }

        private Task<Compilation> GetCompilation()
        {
            var testSource = MvcTestSource.Read(GetType().Name, "ApiConventionAnalyzerTestFile");
            var project = DiagnosticProject.Create(GetType().Assembly, new[] { testSource.Source });

            return project.GetCompilationAsync();
        }
    }
}
