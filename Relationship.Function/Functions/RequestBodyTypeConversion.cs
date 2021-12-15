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

namespace Function.MSABackEnd.Functions
{
    public class RequestBodyTypeConversion
    {
        [Function("RequestBodyTypeConversion")]
        [OpenApiOperation(operationId: "RequestBodyTypeConversion", tags: new[] { "name" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiRequestBody("application/json", typeof(RequestBodyModelTypeConversionTest),
              Description = "JSON request body containing RequestBodyModelTypeConversionTest object")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(string),
              Description = "The OK response message containing a JSON result.")]
        public async Task<HttpResponseData> Run(
              [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequestData req,
            FunctionContext executionContext)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            int? intType = data?.cSharpInt;
            long? longType = data?.cSharpLong;
            decimal? decimalType = data?.cSharpDecimal;
            float? floatType = data?.cSharpFloat;
            
            bool? boolType = data?.cSharpBool;
            string? stringType = data?.cSharpString;
            DateTime? dateTimeType = data?.cSharpDateTime;

            //these data types aren't supported by 
            //the rtk query OpenAPI generator so don't use them :)

            //char? charType = data?.cSharpChar;
            //DateOnly? dateOnly = data?.cSharpDateOnly;
            //TimeOnly? timeOnly = data?.cSharpTimeOnly;

            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "application/json");
            await response.WriteStringAsync(JsonConvert.SerializeObject(new
            {
                intConverted = intType,
                longConverted = longType,
                decimalConverted = decimalType,
                floatConverted = floatType,
                boolConverted = boolType,
                stringConverted = stringType,
                DateTimeConverted = dateTimeType,
            }));

            return response;
        }
    }
    public class RequestBodyModelTypeConversionTest
    {
        public int CSharpInt { get; set; }
        public long CSharpLong { get; set; }
        public decimal CSharpDecimal { get; set; }
        public float CSharpFloat { get; set; }
        public double CSharpDouble { get; set; }
       
        public bool CSharpBool { get; set; }
        public string CSharpString { get; set; }
        public DateTime CSharpDate { get; set; }
        //not supported by rtk query generator
        //public char CSharpChar { get; set; }
        //public DateOnly CSharpDateOnly { get; set; }
        //public TimeOnly CSharpTimeOnly { get; set; }
    }
}
