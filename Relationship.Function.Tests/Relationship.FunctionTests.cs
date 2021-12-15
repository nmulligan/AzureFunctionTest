using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Azure.Functions.Worker;
using Moq;
using Microsoft.Azure.Functions.Worker.Http;
using System;
using System.IO;
using System.Collections.Generic;
using System.Security.Claims;
using System.Net;
using Function.MSABackEnd.Functions;
using Microsoft.Extensions.Logging.Abstractions;
using System.Threading;

namespace Function.Tests
{

    public class FakeHttpRequestData : HttpRequestData
    {
        public FakeHttpRequestData(FunctionContext functionContext, Uri url = null, Stream body = null) : base(functionContext)
        {
            //Url = url;
            //Body = body ?? new MemoryStream();
        }

        public override Stream Body { get; } = new MemoryStream();

        public override HttpHeadersCollection Headers { get; } = new HttpHeadersCollection();

        public override IReadOnlyCollection<IHttpCookie> Cookies { get; }

        public override Uri Url { get; }

        public override IEnumerable<ClaimsIdentity> Identities { get; }

        public override string Method { get; }

        public override HttpResponseData CreateResponse()
        {
            return new FakeHttpResponseData(FunctionContext);
        }
    }

    public class FakeHttpResponseData : HttpResponseData
    {
        public FakeHttpResponseData(FunctionContext functionContext) : base(functionContext)
        {
        }

        public override HttpStatusCode StatusCode { get; set; }
        public override HttpHeadersCollection Headers { get; set; } = new HttpHeadersCollection();
        public override Stream Body { get; set; } = new MemoryStream();
        public override HttpCookies Cookies { get; }
    }

    [TestClass]
    public class FunctionsTests
    {
        private readonly ILogger logger = TestFactory.CreateLogger();

        [TestMethod]
        public async Task Http_trigger_should_return_known_string()
        {
            Mock<FunctionContext> contextMock = new Mock<FunctionContext>();
            FunctionContext context = contextMock.Object;
            var request = new FakeHttpRequestData(context);
            //var logger = new NullLogger<HelloFunction>();
            var helloFunction = new HelloFunction();
            var response = await helloFunction.RunAsync(request, "John", 30, context, logger);
            Assert.AreEqual("Hello, Bill. This HTTP triggered function executed successfully.", response.Body);
        }

        //[MemberData(nameof(TestFactory.Data), MemberType = typeof(TestFactory))]
        public async Task Http_trigger_should_return_known_string_from_member_data(string queryStringKey, string queryStringValue)
        {
            Mock<FunctionContext> contextMock = new Mock<FunctionContext>();
            FunctionContext context = contextMock.Object;
            Mock<HttpRequestData> requestDataMock = new Mock<HttpRequestData>();
            HttpRequestData requestData = requestDataMock.Object;
            var helloFunction = new MSABackEnd.Functions.HelloFunction();
            var request = TestFactory.CreateHttpRequest(queryStringKey, queryStringValue);
            var response = await helloFunction.RunAsync(requestData, "John", 30, context);
            Assert.AreEqual($"Hello, {queryStringValue}. This HTTP triggered function executed successfully.", response.Body);
        }

        //public async Task Timer_should_log_message()
        //{
        //    Mock<FunctionContext> contextMock = new Mock<FunctionContext>();
        //    FunctionContext context = contextMock.Object;
        //    var timeFunction = new MSABackEnd.Functions.TimeFunction();
        //    var logger = (ListLogger)TestFactory.CreateLogger(LoggerTypes.List);
        //    await timeFunction.Run(null, context);
        //    var msg = logger.Logs[0];
        //    Assert.IsTrue(msg.Contains("C# Timer trigger function executed at"));
        //}
    }
}