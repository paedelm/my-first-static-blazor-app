using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.CosmosDB;
using Azure.Data.Tables;
//using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Logging;
using BlazorApp.Shared;
using System.Text.Json;
using System.Collections.Generic;

namespace BlazorApp.Api
{
    public static class StoreUser
    {
        [FunctionName("StoreUser")]
        //[StorageAccount("TableConnection")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] ClientPrincipal principal,
            [CosmosDB(
                databaseName: "ClientPrincipal",
                collectionName: "Principals",
                ConnectionStringSetting = "CosmosDbConnection",
                CreateIfNotExists = true,
                Id = "/UserId",
                PartitionKey = "/UserDetails"
                //Id = { principal.UserId } 
            ) ] IAsyncCollector<ClientPrincipal> principalsOut,
            [CosmosDB(
                databaseName: "ClientPrincipal",
                collectionName: "Principals",
                ConnectionStringSetting = "CosmosDbConnection",
                //Id ="7eeb9cc5-c598-4375-a2a8-6457e4cf9a8f",
                //PartitionKey = "paedelm"
                SqlQuery = "SELECT top 1 * FROM c order by c._ts desc"
            ) ] IEnumerable<ClientPrincipal> lastPrincipals,
            //[Table("ClientPrincipal", "{principal?.UserDetails}", "{principal?.UserId}", Connection = "TableConnection")] MyEntity entity
            ILogger log)
        {
            ClientPrincipal lastPrincipal = new ClientPrincipal();
            foreach (var lp in lastPrincipals) lastPrincipal = lp;
            log.LogInformation($"C# HTTP trigger function processed a request., principal={principal?.UserDetails}");
            log.LogInformation($"lastPrincipal {lastPrincipal.UserId}, {lastPrincipal.UserDetails}");
            var cnct = System.Environment.GetEnvironmentVariable("TableConnection");
            var table = new TableClient(new System.Uri (cnct));
            var x = JsonSerializer.Serialize(principal);
            var y = JsonSerializer.Deserialize<IDictionary<string, object>>(x);
            y["UserRoles"] = string.Join(',',y["UserRoles"]);
            //var dict = Microsoft.Azure.Cosmos.Table.TableEntity.Flatten(principal, new OperationContext());
            log.LogInformation($"tablecnct: {cnct}"); // info: {table.AccountName}, {table}");
            //var entity = new DynamicTableEntity(principal.IdentityProvider, principal.UserId, null,
            //    Microsoft.Azure.Cosmos.Table.TableEntity.Flatten(principal, new OperationContext()));
            var entity = new TableEntity(y);
            var z = table.GetEntity<TableEntity>(principal.IdentityProvider, principal.UserId);
            entity.PartitionKey = principal.IdentityProvider;
            entity.RowKey = principal.UserId;
            foreach (var (key, value) in y) 
                log.LogInformation($"key = {key} value = {value}"); // info: {table.AccountName}, {table}");

            var sEntity = new TableEntity(principal.IdentityProvider, principal.UserId) {
                {"UserId", principal.UserId },
                {"UserDetails", principal.UserDetails },
                {"IdentityProvider", principal.IdentityProvider }
            };
            if (z == null)
            {
                table.AddEntity(entity);
            } else {
                entity.TryAdd("NewProp", "newprop");
                table.UpdateEntity(entity, z.Value.ETag, TableUpdateMode.Replace);
            }
            await principalsOut.AddAsync(principal);
 //           await Task.Delay(0);
            return new OkObjectResult(principal);
        }
    }
}
