using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using UFIDA.U8.Portal.ZT.VouchPlugIns.Models;
using UFIDA.U8.Portal.ZT.VouchPlugIns.Tools;
using UFIDA.U8.UAP.Plus.Common.Util;

namespace UFIDA.U8.Portal.ZT.VouchPlugIns
{
    public class EKBAPIClient
    {

        /// <summary>
        /// 获取TOKEN
        /// </summary>
        /// <returns></returns>
        public static bool GetAccessToken() 
        {
            Constant.token = "";
            if (CacheHelper.CacheKeyContains("YKBToken"))
            {
                var cache = CacheHelper.GetCache("YKBToken");
                Constant.token = cache.ToString();
                return true;
            }
            else
            {
                Dictionary<string, string> dicParas = new Dictionary<string, string>();
                object paras = new
                {
                    appKey = Constant.appKey,
                    appSecurity = Constant.appSecurity,
                };
                string Jparas = JsonConvert.SerializeObject(paras);
                dicParas.Add("data", Jparas);

                var eResult = ClientHelper.HttpData("POST", Constant.urlGetToken, null, dicParas);
                JObject Jresult = JsonHelper.GetJObject(eResult, "", true);
                string token = Jresult["value"]?["accessToken"]?.ToString();
                if (!string.IsNullOrEmpty(token))
                {
                    long timestamp = _c.ToLong(Jresult["value"]["expireTime"]);
                    CacheHelper.SetCache("YKBToken",token,timestamp);
                    Constant.token = token;
                    return true;
                }
                else
                {
                    MessageBox.Show("获取易快报方TOKEN失败，详细错误信息请查看日志。");
                    MsgUtil.WriteFile($"GetAccessToken Error> 参数{Jparas}，接口返回的信息:{eResult}");
                }
            }


            return false;
        }


        /// <summary>
        /// 获取员工ID与归属部门ID
        /// </summary>
        /// <param name="personCode"></param>
        /// <returns></returns>
        public static RtnPersonInfo GetPersonInfo(string personCode) 
        {
            RtnPersonInfo rtnInfo = null;
            Dictionary<string, string> dicParas = new Dictionary<string, string>();
            object paras = new
            {
                type = "CODE",
                conditionIds = new List<string>() { personCode }
            };
            string Jparas = JsonConvert.SerializeObject(paras);
            dicParas.Add("data", Jparas);
            var eResult = ClientHelper.HttpData("POST", Constant.urlGetStaffInfo+$"{Constant.token}", null, dicParas);
            JObject Jresult = JsonHelper.GetJObject(eResult);
            var hasValues = Jresult["items"]?.HasValues;
            MsgUtil.WriteFile($"开始查询YKB人员信息：返回结果:{eResult},标识:{hasValues}");
            if (hasValues != null && (bool)hasValues)
            {
                string id = Jresult["items"]?[0]?["id"]?.ToString();
                if (!string.IsNullOrEmpty(id))
                {
                    rtnInfo = JsonConvert.DeserializeObject<RtnPersonInfo>(eResult);
                    MsgUtil.WriteFile($"业务员YKBID：{id} ");
                    return rtnInfo;
                }
            }
            else
            {
                MessageBox.Show($"未通过业务员[{personCode}]找到易快报相关的档案信息，请确认该人员是否已在易快报建档！");
                MsgUtil.WriteFile($"GetPersonInfo Error> 参数{Jparas}，接口返回的信息:{eResult}");
            }

            return null;

        }

        /// <summary>
        /// 获取模板ID
        /// </summary>
        /// <returns></returns>
        public static string GetTemplateId(string url)
        {
            string templateId = "";
            var eResult = ClientHelper.HttpData("GET", url + $"{Constant.token}", null, null);
            JObject Jresult = JsonHelper.GetJObject(eResult);
            var hasValues = Jresult["items"]?.HasValues;
            if (hasValues != null && (bool)hasValues)
            {
                templateId = Jresult["items"]?[0]?["id"]?.ToString();
                MsgUtil.WriteFile($"模板id:{templateId}");
                return templateId;
            }
            else
            {
                MessageBox.Show($"获取模板Id信息列表失败，详细错误信息请查看日志。");
                MsgUtil.WriteFile($"GetTemplateId Error> ，接口返回的信息:{eResult}");
            }

            return string.Empty;
        }

        /// <summary>
        /// 推送单据到易快报合同
        /// </summary>
        /// <param name="contract"></param>
        /// <returns></returns>
        public static bool CreateContract(Contract contract) 
        {
            if (contract!=null)
            {
                try
                {
                    Dictionary<string, string> dicParas = new Dictionary<string, string>();
                    string Jparas = JsonConvert.SerializeObject(contract);
                    dicParas.Add("data", Jparas);
                    string url = $"{Constant.urlCreateContract}{Constant.token}&isCommit=true&isUpdate=false";
                    var eResult = ClientHelper.HttpData("POST", url, null, dicParas);                    
                    JObject Jresult = JsonHelper.GetJObject(eResult);
                    string id = Jresult["flow"]?["id"]?.ToString();
                    if (!string.IsNullOrEmpty(id))
                    {
                        MsgUtil.WriteFile($"开始请求易快报合同创建接口，url:{Constant.urlCreateContract + $"{Constant.token}"},参数:{Jparas}，接口返回的id:{id}");
                        return true;
                    }
                    else
                    {

                        MessageBox.Show($"创建易快报合同失败，错误原因：{eResult}");
                        MsgUtil.WriteFile($"请求易快报合同创建接口失败，url:{Constant.urlCreateContract + Constant.token},参数:{Jparas}，接口返回的信息:{eResult}");
                    }
                }
                catch (Exception ex)
                {

                    throw;
                }
            }


            return false;
        }
    }
}
