using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using BlazorApp.Shared;
//using System.Net;
//using Microsoft.AspNetCore.Authorization;
//using System.Security.Claims;

namespace BlazorApp.Api
{
    public static class TestFunc
    {
        [FunctionName("TestFunc")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request for function TestFunc.");
            var principal = StaticWebAppsAuth.Parse(req);
            if (principal.Identity == null) return StaticWebAppsAuth.Forbidden(errorMessage: "Fout 403: Niet Toegestaan");

            // var identName = principal.Identity == null ? "anonymous" : principal.Identity.Name;
            var identName = principal.Identity.Name;
            log.LogInformation($"principal: {identName}");
            string name = req.Query["name"];
            var kenmerk = Environment.GetEnvironmentVariable("kenmerk") ?? string.Empty;

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            name = name ?? data?.name;
            var responseMessage = string.IsNullOrEmpty(name)
                ? new NaamBericht(Naam: "Nobody", Bericht: $"{kenmerk} principal:{identName}:This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response.")
                : new NaamBericht(Naam: name, Bericht: $"{kenmerk} principal: {identName}:Hello, {name}. This HTTP triggered function executed successfully.");
            return new OkObjectResult(responseMessage);
        }
    }
}
