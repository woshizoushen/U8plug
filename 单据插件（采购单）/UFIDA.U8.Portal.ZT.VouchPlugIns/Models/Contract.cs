using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UFIDA.U8.Portal.ZT.VouchPlugIns.Models
{
    public class Contract
    {
        [JsonProperty("form", NullValueHandling = NullValueHandling.Ignore)]
        public Form form;

        public class Form
        {
            [JsonProperty("outerCode", NullValueHandling = NullValueHandling.Ignore)]
            public string outerCode;

            [JsonProperty("specificationId", NullValueHandling = NullValueHandling.Ignore)]
            public string specificationId;

            [JsonProperty("u_制单时间", NullValueHandling = NullValueHandling.Ignore)]
            public long cMakeTime;

            [JsonProperty("u_合同编码", NullValueHandling = NullValueHandling.Ignore)]
            public string contractCode;

            [JsonProperty("u_合同名称", NullValueHandling = NullValueHandling.Ignore)]
            public string contractName;

            [JsonProperty("u_项目名称", NullValueHandling = NullValueHandling.Ignore)]
            public string cItemName;

            [JsonProperty("u_项目编码", NullValueHandling = NullValueHandling.Ignore)]
            public string cItemCode;

            [JsonProperty("u_合同类型", NullValueHandling = NullValueHandling.Ignore)]
            public string contractType;

            [JsonProperty("u_合同性质", NullValueHandling = NullValueHandling.Ignore)]
            public string contractKind;

            [JsonProperty("u_主合同编码", NullValueHandling = NullValueHandling.Ignore)]
            public string contractMainCode;

            [JsonProperty("u_对方单位", NullValueHandling = NullValueHandling.Ignore)]
            public string cVenName;

            [JsonProperty("u_对方负责人", NullValueHandling = NullValueHandling.Ignore)]
            public string cVenContactMethod;

            [JsonProperty("u_合同签订日期", NullValueHandling = NullValueHandling.Ignore)]
            public long DateSign;

            [JsonProperty("u_合同开始日期", NullValueHandling = NullValueHandling.Ignore)]
            public long DateBegin;

            [JsonProperty("u_合同结束日期", NullValueHandling = NullValueHandling.Ignore)]
            public long DateEnd;

            [JsonProperty("u_生效日期", NullValueHandling = NullValueHandling.Ignore)]
            public long DateEffective;

            [JsonProperty("u_合同币种", NullValueHandling = NullValueHandling.Ignore)]
            public string currency;

            [JsonProperty("u_汇率", NullValueHandling = NullValueHandling.Ignore)]
            public string exchangeRate;

            [JsonProperty("u_收付款计划方向", NullValueHandling = NullValueHandling.Ignore)]
            public string planDirection;

            [JsonProperty("u_部门1", NullValueHandling = NullValueHandling.Ignore)]
            public string Department;

            [JsonProperty("submitterId", NullValueHandling = NullValueHandling.Ignore)]
            public string submitterId;

            [JsonProperty("u_业务类型", NullValueHandling = NullValueHandling.Ignore)]
            public string busType;

            [JsonProperty("u_合同的总数量", NullValueHandling = NullValueHandling.Ignore)]
            public string iQua;

            [JsonProperty("u_合同的总金额", NullValueHandling = NullValueHandling.Ignore)]
            public Money natSum;

            [JsonProperty("u_合同的原币金额", NullValueHandling = NullValueHandling.Ignore)]
            public Money sum;

            [JsonProperty("u_合同描述", NullValueHandling = NullValueHandling.Ignore)]
            public string memo;
        }



        public class Money
        {
            [JsonProperty("standard")]
            public string standard;

            [JsonProperty("standardUnit")]
            public string standardUnit = "元";

            [JsonProperty("standardScale")]
            public int standardScale = 2;

            [JsonProperty("standardSymbol")]
            public string standardSymbol = "¥";

            [JsonProperty("standardNumCode")]
            public string standardNumCode = "156";

            [JsonProperty("standardStrCode")]
            public string standardStrCode = "CNY";
        }

    }
}
