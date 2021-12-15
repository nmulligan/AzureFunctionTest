using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
//using Isolated.Business.Services;

namespace Function.MSABackEnd.Functions
{
    public class TimeFunction
    {
        //commented out as this interface
        //should existed in a referenced project
        //and not reside in this project.
        //private ITimeService _timeService;

        //public TimeFunction(ITimeService timeService)
        //{
        //    _timeService = timeService;
        //}
        [Function("TimeGet")]
        [OpenApiOperation(operationId: "TimeGet", tags: new[] { "name" }, Deprecated = false, Description = "Decoration pattern for function with no parameters", Summary = "", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(string), Description = "The OK response")]
        public async Task<HttpResponseData> Run(
               [HttpTrigger(AuthorizationLevel.Function, "post", Route = "time")] HttpRequestData req,
            FunctionContext executionContext)

        {
            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "application/json");
            //await response.WriteStringAsync(JsonConvert.SerializeObject(new { response = _timeService.GetCurrentTime() })); ;
            await response.WriteStringAsync(JsonConvert.SerializeObject(new { response = DateTime.Now })); ;
            return response;
        }
    }
}
