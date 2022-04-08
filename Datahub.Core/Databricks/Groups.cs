using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Datahub.Core.Databricks
{

    public class GroupMember
    {
        public string display { get; set; }
        public string value { get; set; }

        [JsonPropertyName("$ref")]
        public string Ref { get; set; }
    }

    public record DatabricksGroupEntitlement
    {
        public string value { get; set; }
    }

    public class DatabricksGroup
    {
        public List<DatabricksGroupEntitlement> entitlements { get; set; }
        public string displayName { get; set; }
        public List<GroupMember> members { get; set; }
        public List<object> groups { get; set; }
        public string id { get; set; }
    }

    public class DatabricksGroups
    {
        public int totalResults { get; set; }
        public int startIndex { get; set; }
        public int itemsPerPage { get; set; }
        public List<string> schemas { get; set; }
        public List<DatabricksGroup> Resources { get; set; }
    }


}
