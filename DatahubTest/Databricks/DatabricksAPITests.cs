using Azure.Identity;
using Datahub.Core.Databricks;
using Microsoft.Azure.Management.ResourceManager.Fluent.Authentication;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.PowerBI.Api;
using Microsoft.PowerBI.Api.Models;
using Microsoft.Rest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace Datahub.Tests.Databricks
{
    public class DatabricksAPITests
    {
        //Resource Uri for Power BI API
        private const string resourceUri = "https://analysis.windows.net/powerbi/api";

        private const string authorityUri = "https://login.windows.net/common/oauth2/authorize";

        private const string graphUri = "https://graph.microsoft.com/.default";


        public const string DATABRICKS_INSTANCE = "https://adb-588851212245547.7.azuredatabricks.net";

        //programmatic ID for Azure Databricks 
        //https://docs.microsoft.com/en-us/azure/databricks/dev-tools/api/latest/aad/app-aad-token
        public const string DATABRICKS_SCOPE = "2ff814a6-3304-4ab8-85cb-cd0e6f879c1d/.default";


        public const string USERS_API = "/api/2.0/preview/scim/v2/Users";
        public const string GROUPS_API = "/api/2.0/preview/scim/v2/Groups";

        [Fact]
        public async Task GivenCurrentUser_ListDatabricksUsers()
        {
            var httpClient = await DatabricksRequestClient();
            //Assert.True(allDataSets.Count(d => d.EndorsementDetails != null) > 0);
            var userData = await JsonSerializer.DeserializeAsync<DatabricksUsers>(await httpClient.GetStreamAsync(USERS_API));
            Assert.NotNull(userData);
            var entitlements = userData.Resources.Where(u => u.entitlements != null)
                .SelectMany(u => u.entitlements)
                .SelectMany(e => e.value).Distinct().ToList();
            Assert.Equal(5, entitlements.Count);
        }

        [Fact]
        public async Task GivenCurrentUser_ListDatabricksGroups()
        {
            var httpClient = await DatabricksRequestClient();
            //Assert.True(allDataSets.Count(d => d.EndorsementDetails != null) > 0);
            //var groupsJson = await httpClient.GetStringAsync(GROUPS_API);
            var groupData = await JsonSerializer.DeserializeAsync<DatabricksGroups>(await httpClient.GetStreamAsync(GROUPS_API));
            Assert.NotNull(groupData);

        }

        private static async Task<HttpClient> DatabricksRequestClient()
        {
            var tenant = "8c1a4d93-d828-4d0e-9303-fd3bd611c822";
            var credential = new DefaultAzureCredential(
                new DefaultAzureCredentialOptions()
                {
                    InteractiveBrowserTenantId = tenant,
                    VisualStudioCodeTenantId = tenant,
                    VisualStudioTenantId = tenant,
                    SharedTokenCacheTenantId = tenant,
                    ExcludeInteractiveBrowserCredential = false,
                    ExcludeVisualStudioCodeCredential = true
                });
            var token = await credential.GetTokenAsync(new Azure.Core.TokenRequestContext(new[] { DATABRICKS_SCOPE }));
            int timeoutSeconds = 30;
            var httpClient = new HttpClient(new HttpClientHandler
            {
                AutomaticDecompression = (DecompressionMethods.GZip | DecompressionMethods.Deflate)
            }, disposeHandler: false)
            {
                BaseAddress = new Uri(DATABRICKS_INSTANCE),
                Timeout = TimeSpan.FromSeconds(timeoutSeconds)
            };
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Token);
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
            
            return httpClient;
        }

        [Fact]
        public async Task GivenCurrentUser_CreateUser()
        {
            using var httpClient = await DatabricksRequestClient();
            var groupData = await JsonSerializer.DeserializeAsync<DatabricksGroups>(await httpClient.GetStreamAsync(GROUPS_API));
            var users = groupData.Resources.FirstOrDefault(g => g.displayName == "users");

            var userRequest = new DatabricksUser()
            {
                userName = "<user>@ssc-spc.gc.ca",
                groups = new() { new() { value = users.id } },
                entitlements = new() { new() { value = "workspace-access" } }
            };
            var opts = new JsonSerializerOptions() { IgnoreNullValues =  true };
            var jsonText = JsonSerializer.Serialize(userRequest,opts);

            var jsonContent = new StringContent(jsonText, Encoding.UTF8, "application/scim+json");
            var response = await httpClient.PostAsync(USERS_API, jsonContent);

            Assert.NotNull(response);
        }
    }
}
