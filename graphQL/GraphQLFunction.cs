using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using BlazorApp.Shared;

namespace graphQL;

public class GraphQLFunction
{
    [FunctionName("GraphQLHttpFunction")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "graphql/{**slug}")]
        HttpRequest request,
        ILogger log,
        [GraphQL]
        IGraphQLRequestExecutor executor)
    {
        log.Log(LogLevel.Error, $"request={request.Body}");
        return new OkObjectResult(new NaamBericht(Naam: "GraphQL", Bericht: "Zou dit dan wel werken?"));
        //return await executor.ExecuteAsync(request);
    }
}