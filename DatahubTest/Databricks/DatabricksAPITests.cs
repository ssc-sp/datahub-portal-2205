using Azure.Identity;
using Microsoft.Azure.Management.ResourceManager.Fluent.Authentication;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.PowerBI.Api;
using Microsoft.PowerBI.Api.Models;
using Microsoft.Rest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
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

        public const string USERS_API = "/api/2.0/preview/scim/v2/Users";

        [Fact]
        public async Task GivenCurrentUser_ListDatabricksUsers()
        {

            var handler = new HttpClientHandler();
            handler.UseDefaultCredentials = true;

            var credential = new DefaultAzureCredential(new DefaultAzureCredentialOptions() { ExcludeInteractiveBrowserCredential = false, ExcludeVisualStudioCodeCredential = true  });
            var token = await credential.GetTokenAsync(
                new Azure.Core.TokenRequestContext(
                    new[] { DATABRICKS_INSTANCE })
                );

            var serviceCredentials = new TokenCredentials(token.Token, "Bearer");
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = 
            //Assert.True(allDataSets.Count(d => d.EndorsementDetails != null) > 0);
        }
    }
}
