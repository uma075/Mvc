using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace Microsoft.AspNetCore.Mvc.Analyzers
{
    public class UnwrapMethodReturnType
    {
        public ApiConventionAnalyzerBaseModel ReturnsBaseModel() => null;

        public ActionResult<ApiConventionAnalyzerBaseModel> ReturnsActionResultOfBaseModel() => null;

        public Task<ActionResult<ApiConventionAnalyzerBaseModel>> ReturnsTaskOfActionResultOfBaseModel() => null;

        public ValueTask<ActionResult<ApiConventionAnalyzerBaseModel>> ReturnsValueTaskOfActionResultOfBaseModel() => default(ValueTask<ActionResult<ApiConventionAnalyzerBaseModel>>);

        public ActionResult<IEnumerable<ApiConventionAnalyzerBaseModel>> ReturnsActionResultOfIEnumerableOfBaseModel() => null;

        public IEnumerable<ApiConventionAnalyzerBaseModel> ReturnsIEnumerableOfBaseModel() => null;
    }

    [DefaultStatusCode(StatusCodes.Status412PreconditionFailed)]
    public class TestActionResultUsingStatusCodesConstants { }

    [DefaultStatusCode((int)HttpStatusCode.EarlyHints)]
    public class TestActionResultUsingHttpStatusCodeCast { }

    public class ApiConventionAnalyzerBaseModel { }

    public class ApiConventionAnalyzerDerivedModel : ApiConventionAnalyzerBaseModel { }
}
