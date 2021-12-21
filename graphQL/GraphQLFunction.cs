using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;

namespace graphQL;

public class GraphQLFunction
{
    [FunctionName("graphql")]
    public Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "graphql/{**slug}")] 
        HttpRequest request,
        [GraphQL] 
        IGraphQLRequestExecutor executor)
        => executor.ExecuteAsync(request);
}