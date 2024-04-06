using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UFIDA.U8.Portal.ZT.VouchPlugIns.Models
{
    public class RtnPersonInfo
    {
        [JsonProperty("items", NullValueHandling = NullValueHandling.Ignore)]
        public List<Item> items { get; set; }



        public class Item
        {
            [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
            public string id { get; set; }

            [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
            public string name { get; set; }

            [JsonProperty("code", NullValueHandling = NullValueHandling.Ignore)]
            public string code { get; set; }

            [JsonProperty("departments", NullValueHandling = NullValueHandling.Ignore)]
            public List<string> departments { get; set; }

            [JsonProperty("defaultDepartment", NullValueHandling = NullValueHandling.Ignore)]
            public string defaultDepartment { get; set; }

            [JsonProperty("cellphone", NullValueHandling = NullValueHandling.Ignore)]
            public string cellphone { get; set; }
            [JsonProperty("active", NullValueHandling = NullValueHandling.Ignore)]
            public bool active { get; set; }

            [JsonProperty("userId", NullValueHandling = NullValueHandling.Ignore)]
            public string userId { get; set; }

            [JsonProperty("email", NullValueHandling = NullValueHandling.Ignore)]
            public object email { get; set; }

            [JsonProperty("showEmail", NullValueHandling = NullValueHandling.Ignore)]
            public object showEmail { get; set; }

            [JsonProperty("external", NullValueHandling = NullValueHandling.Ignore)]
            public bool external { get; set; }

            [JsonProperty("authState", NullValueHandling = NullValueHandling.Ignore)]
            public bool authState { get; set; }

            [JsonProperty("globalRoaming", NullValueHandling = NullValueHandling.Ignore)]
            public string globalRoaming { get; set; }

            [JsonProperty("nickName", NullValueHandling = NullValueHandling.Ignore)]
            public object nickName { get; set; }

            [JsonProperty("note", NullValueHandling = NullValueHandling.Ignore)]
            public object note { get; set; }

            [JsonProperty("staffCustomForm", NullValueHandling = NullValueHandling.Ignore)]
            public StaffCustomForm staffCustomForm { get; set; }

            [JsonProperty("updateTime", NullValueHandling = NullValueHandling.Ignore)]
            public string updateTime { get; set; }

            [JsonProperty("createTime", NullValueHandling = NullValueHandling.Ignore)]
            public string createTime { get; set; }

            [JsonProperty("roles", NullValueHandling = NullValueHandling.Ignore)]
            public List<object> roles { get; set; }
        }



        public class StaffCustomForm
        {
            [JsonProperty("certificate", NullValueHandling = NullValueHandling.Ignore)]
            public List<object> certificate { get; set; }

            [JsonProperty("defaultDepartment.form.costCenter", NullValueHandling = NullValueHandling.Ignore)]
            public string defaultDepartmentformcostCenter { get; set; }

            [JsonProperty("defaultDepartment.form.legalEntity", NullValueHandling = NullValueHandling.Ignore)]
            public string defaultDepartmentformlegalEntity { get; set; }
        }
    }
}
