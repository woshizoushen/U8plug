using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UFIDA.U8.Portal.ZT.VouchPlugIns.Tools
{
    public class Constant
    {
        public static string token = "";

        public static string appKey = "";

        public static string appSecurity = "";

        public static string urlGetToken = "https://dd2.ekuaibao.com/api/openapi/v2.1/auth/getAccessToken";

        public static string urlGetStaffInfo = "https://dd2.ekuaibao.com/api/openapi/v2.1/staffs/getStaffIds?accessToken=";

        public static string urlGetContractTemplateId = "https://dd2.ekuaibao.com/api/openapi/v2/specifications/byIds/[ID01v9oCp7PwMn]?accessToken=";

        public static string urlCreateContract = "https://dd2.ekuaibao.com/api/openapi/v2.2/flow/data?accessToken=";

        public static string urlCreatePayInfo = "https://dd2.ekuaibao.com/api/openapi/v2.1/payeeInfos?accessToken=";

        public static string urlSearchPayInfo = "https://dd2.ekuaibao.com/api/openapi/v2/payeeInfos?start=0&count=100&active=true&accessToken=";

        public static string urlGetDepId = "https://dd2.ekuaibao.com/api/openapi/v1/departments/getDepartmentByName?accessToken=";

        public static string urlGetCurrency = "https://dd2.ekuaibao.com/api/openapi/v2.1/currency?accessToken=";
    }
}
