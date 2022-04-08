using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Datahub.Core.Databricks
{
    public class DatabricksEmail
    {
        public string type { get; set; }
        public string value { get; set; }
        public bool primary { get; set; }
    }

    public class DatabricksUserGroup
    {
        public string display { get; set; }
        public string type { get; set; }
        public string value { get; set; }

        [JsonPropertyName("$ref")]
        public string Ref { get; set; }
    }

    public class DatabricksEntitlement
    {
        public string value { get; set; }
    }

    public class DatabricksUserName
    {
        public string familyName { get; set; }
        public string givenName { get; set; }
    }

    public class DatabricksUser
    {
        public string[] schemas => new[] { "urn:ietf:params:scim:schemas:core:2.0:User" };
        public List<DatabricksEmail> emails { get; set; }
        public bool? active { get; set; }
        public List<DatabricksUserGroup> groups { get; set; }
        public string id { get; set; }
        public string userName { get; set; }
        public List<DatabricksEntitlement> entitlements { get; set; }
        public string externalId { get; set; }
        public string displayName { get; set; }
        public DatabricksUserName? name { get; set; }
    }

    public class DatabricksUsers
    {
        public int totalResults { get; set; }
        public int startIndex { get; set; }
        public int itemsPerPage { get; set; }
        public List<string> schemas { get; set; }
        public List<DatabricksUser> Resources { get; set; }
    }

}
