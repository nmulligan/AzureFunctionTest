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
    public class RequestBodyFunction
    {
        const double revenuePerkW = 0.12;
        const double technicianCost = 250;
        const double turbineCost = 100;

        [Function("RequestBodyFunction")]
        [OpenApiOperation(operationId: "RequestBodyFunction")]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiRequestBody("application/json", typeof(RequestBodyModel),
            Description = "JSON request body containing { hours, capacity}")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(string),
            Description = "The OK response message containing a JSON result.")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequestData req,
            FunctionContext executionContext)
        {
            // Get request body data.
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            int? capacity = data?.capacity;
            int? hours = data?.hours;

            // Return bad request if capacity or hours are not passed in
            if (capacity == null || hours == null)
            {
                var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                await badResponse.WriteStringAsync("Please pass capacity and hours in the request body").ConfigureAwait(false);
                return badResponse;
            }
            // Formulas to calculate revenue and cost
            double? revenueOpportunity = capacity * revenuePerkW * 24;
            double? costToFix = (hours * technicianCost) + turbineCost;
            string repairTurbine;

            if (revenueOpportunity > costToFix)
            {
                repairTurbine = "Yes";
            }
            else
            {
                repairTurbine = "No";
            };
            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "application/json");
            await response.WriteStringAsync(JsonConvert.SerializeObject(new
            {
                message = repairTurbine,
                revenueOpportunity = "$" + revenueOpportunity,
                costToFix = "$" + costToFix
            })).ConfigureAwait(false);
            return response;

        }
    }
    public class RequestBodyModel
    {
        public int Hours { get; set; }
        public int Capacity { get; set; }
    }
}
