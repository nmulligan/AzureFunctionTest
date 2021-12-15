using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

namespace Function.MSABackEnd.Functions
{
    public class HelloFunction
    {
        [Function("HelloGet")]
        [OpenApiOperation(operationId: "HelloGet", tags: new[] { "name" }, Deprecated = false, Description = "Decoration pattern for function with multiple path parameters", Summary = "", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiParameter(name: "name", In = ParameterLocation.Path, Required = true, Type = typeof(string), Description = "The **Name** path route")]
        [OpenApiParameter(name: "age", In = ParameterLocation.Path, Required = true, Type = typeof(int), Description = "The **age** path route")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(Client),
           Description = "The OK response message containing a JSON result.")]
        public async Task<HttpResponseData> RunAsync(
           [HttpTrigger(AuthorizationLevel.Function, "get", Route = "hello/{name}/{age}")] HttpRequestData req,
           string name,
           int age, FunctionContext executionContext, ILogger logger = null)
        {

            if (string.IsNullOrEmpty(name))
                return req.CreateResponse(HttpStatusCode.BadRequest);

            if(logger == null)
            {
                logger = executionContext.GetLogger<HelloFunction>();
            }
            logger.LogInformation("this is a log");

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync<Client>(new Client() { FirstName = "jeff", LastName = "smith", Id = 1 }).ConfigureAwait(false); 
            return response;
        }
    }
    public class Client
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

    }

    
}
