using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace Function.MSABackEnd.Functions
{
    public class QueryParameterFunction
    {
        [Function("QueryParmeterFunction")]
        [OpenApiOperation(operationId: "QueryParmeterFunction", tags: new[] { "name" }, Deprecated = false, Description = "Decoration pattern for function that takes Query string parameter", Summary = "", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiParameter(name: "name", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The **Name** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(string), Description = "The OK response")]
        public async Task<HttpResponseData> Run(
           [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequestData req,
            FunctionContext executionContext)
        {
            executionContext.GetLogger<HelloFunction>().LogInformation("C# HTTP trigger function processed a request.");

            var queries = QueryHelpers.ParseNullableQuery(req.Url.Query);
            string name = queries["name"];



            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            name = name ?? data?.name;

            string responseMessage = string.IsNullOrEmpty(name)
                ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
                : $"Hello, {name}. This HTTP triggered function executed successfully.";

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteStringAsync(responseMessage).ConfigureAwait(false);
            return response;
        }
    }
}
