using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace graphQL;

public class GraphQLFunction
{
    [FunctionName("GraphQLHttpFunction")]
    public Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "graphql/{**slug}")]
        HttpRequest request,
        ILogger log,
        [GraphQL]
        IGraphQLRequestExecutor executor)
    {
        log.Log(LogLevel.Error, $"request={request.Body}");
        return executor.ExecuteAsync(request);
    }
}