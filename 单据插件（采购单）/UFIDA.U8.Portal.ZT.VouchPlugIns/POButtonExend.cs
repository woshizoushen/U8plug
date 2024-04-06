using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using U8Login;
using UFIDA.U8.Portal.ZT.VouchPlugIns.Models;
using UFIDA.U8.Portal.ZT.VouchPlugIns.Tools;
using UFIDA.U8.UAP.Plus.Common.Util;

namespace UFIDA.U8.Portal.ZT.VouchPlugIns
{
    public class POButtonExend : ICommand
    {
        public void AfterRunSysCommand(object objLogin, object objForm, object objVoucher, string sKey, object VarentValue, ref bool Cancel, string other)
        {
            throw new NotImplementedException();
        }

        public void BeforeRunSysCommand(object objLogin, object objForm, object objVoucher, string sKey, object VarentValue, ref bool Cancel, string other)
        {
            throw new NotImplementedException();
        }

        public void Init(object objLogin, object objForm, object objVoucher, object msbar)
        {
            throw new NotImplementedException();
        }

        public void RunCommand(object objLogin, object objForm, object objVoucher, string sKey, object VarentValue, string other)
        {
            clsLogin u8Login = objLogin as clsLogin;
            if (u8Login == null)
            {
                MessageBox.Show("未获取到用户信息!");
                return;
            }

            string db = u8Login.UFDataConnstringForNet;        //数据库连接字符串
            //string[] listSp = db.Split(';');
            //string curStr = listSp[3];
            //int startIndex = curStr.IndexOf("=") + 1;
            //string curDb = curStr.Substring(startIndex, curStr.Length - startIndex);
            string accId = u8Login.get_cAcc_Id();              //账套ACCID
            string usId = u8Login.cUserId;
            string usName = u8Login.cUserName;
            string errMsg = "";


            Type ctlVoucher = Type.GetTypeFromProgID("VoucherControl85.ctlVoucher"); //取得COM组件的类型

            //获取表体数据总行数
            int rows = _c.ToInt(ctlVoucher.InvokeMember("BodyRows", BindingFlags.GetProperty, null, objVoucher, null));
            if (rows < 1)
            {
                MessageBox.Show("未获取到表体信息!");
                return;
            };

            try
            {
                string appKeY= "791d13e0-3585-4921-9ca2-6c41ba0c6955";
                string appSecurity = "25d51fa9-833f-484a-b73c-bb30463fd001";

                if (string.IsNullOrEmpty(appKeY) || string.IsNullOrEmpty(appSecurity))
                {
                    MessageBox.Show("未获取到配置文件信息，请检查配置文件！");
                    return;
                }
                else
                {
                    Constant.appKey = appKeY;
                    Constant.appSecurity = appSecurity;
                }

                var ccode = _c.ToString(ctlVoucher.InvokeMember("HeaderText", BindingFlags.GetProperty, null, objVoucher, new object[] { "cPOID" })); //单号
                var cVenCode= _c.ToString(ctlVoucher.InvokeMember("HeaderText", BindingFlags.GetProperty, null, objVoucher, new object[] { "cVenCode" })); //供应商编码
                var dDate= _c.ToString(ctlVoucher.InvokeMember("HeaderText", BindingFlags.GetProperty, null, objVoucher, new object[] { "dPODate" })); //单据日期
                var cVenPerson= _c.ToString(ctlVoucher.InvokeMember("HeaderText", BindingFlags.GetProperty, null, objVoucher, new object[] { "cVenPerson" })) ?? ""; //供应商联系人
                var cexch_name=  _c.ToString(ctlVoucher.InvokeMember("HeaderText", BindingFlags.GetProperty, null, objVoucher, new object[] { "cexch_name" })); //币种
                var nflat=  _c.ToString(ctlVoucher.InvokeMember("HeaderText", BindingFlags.GetProperty, null, objVoucher, new object[] { "nflat" })); //汇率
                var cDepCode= _c.ToString(ctlVoucher.InvokeMember("HeaderText", BindingFlags.GetProperty, null, objVoucher, new object[] { "cDepCode" })); //部门编码  暂时不用U8部门，通过人员关联YKB部门ID
                var cPersonCode= _c.ToString(ctlVoucher.InvokeMember("HeaderText", BindingFlags.GetProperty, null, objVoucher, new object[] { "cPersonCode" })) ?? ""; //业务员编码
                var cBusType= _c.ToString(ctlVoucher.InvokeMember("HeaderText", BindingFlags.GetProperty, null, objVoucher, new object[] { "cBusType" })); //业务类型
                var cmaketime = _c.ToString(ctlVoucher.InvokeMember("HeaderText", BindingFlags.GetProperty, null, objVoucher, new object[] { "cmaketime" })); //制单时间
                var cAuditDate= _c.ToString(ctlVoucher.InvokeMember("HeaderText", BindingFlags.GetProperty, null, objVoucher, new object[] { "cAuditDate" })) ?? ""; //审核日期
                var cMemo= _c.ToString(ctlVoucher.InvokeMember("HeaderText", BindingFlags.GetProperty, null, objVoucher, new object[] { "cMemo" })); //备注
                var cItemCode = _c.ToString(ctlVoucher.InvokeMember("BodyText", BindingFlags.GetProperty, null, objVoucher, new Object[] { 1, "cItemCode" })) ?? ""; //项目编码
                var cItemName= _c.ToString(ctlVoucher.InvokeMember("BodyText", BindingFlags.GetProperty, null, objVoucher, new Object[] { 1, "cItemName" })) ?? ""; //项目名称


                MsgUtil.WriteFile($"采购订单【{ccode}】开始操作");

                if (string.IsNullOrEmpty(cAuditDate))
                {
                    MessageBox.Show("当前单据还未审核，不能进行推送！");
                    return;
                }


                TargetData dao = new TargetData(db);
                string postatus = dao.GetPOStatus(ccode) ?? "";
                if (!string.IsNullOrEmpty(postatus))
                {
                    if(MessageBox.Show("该单据已创建过易快报合同，是否要继续推送？要继续推送请点击 ‘是’ 并确认已在易快报删除该单据", "U8提示", MessageBoxButtons.YesNo) == DialogResult.No)
                    {
                        return;
                    }
                    
                }

                string jobNumber = dao.GetJobNum(cPersonCode); //根据人员编码获取工号
                if (string.IsNullOrEmpty(jobNumber))
                {
                    MessageBox.Show($"业务员[{cPersonCode}]未设置人员编码!");
                    return;
                }

                int QuaSum = 0;   //总数量
                decimal Sum = 0;    //原币总金额
                decimal NatSum = 0; //本币总金额
                
                for (int i = 1; i <= rows; i++)
                {
                    var iQuantity = _c.ToDecimal(ctlVoucher.InvokeMember("BodyText", BindingFlags.GetProperty, null, objVoucher, new Object[] { i, "iQuantity" })); //数量
                    var iSum= _c.ToDecimal(ctlVoucher.InvokeMember("BodyText", BindingFlags.GetProperty, null, objVoucher, new Object[] { i, "iSum" })); //原币金额
                    var iNatSum = _c.ToDecimal(ctlVoucher.InvokeMember("BodyText", BindingFlags.GetProperty, null, objVoucher, new Object[] { i, "iNatSum" })); //本币金额

                    QuaSum += Convert.ToInt32(iQuantity);
                    Sum += iSum;
                    NatSum += iNatSum;
                }

                EKBAPIClient.GetAccessToken(); //获取并缓存TOKEN
                if (string.IsNullOrEmpty(Constant.token)) 
                {
                    MessageBox.Show("获取EKB token失败，详细错误信息请查看日志！");
                    return;
                }
                RtnPersonInfo personInfo = EKBAPIClient.GetPersonInfo(jobNumber); //获取YKB人员信息列表
                if (personInfo == null) return;
                if (personInfo.items?.Count > 1) //若存在同工号人员，则排除已停用的
                {
                    var items = personInfo.items.Where(a => a.authState == false);
                    if (items != null && items.Count() > 0)
                    {
                        if (items.Count() == personInfo.items.Count())
                        {
                            MessageBox.Show($"员工{personInfo.items[0].name}在易快报中的状态为已停用，无法将该员工作为业务人员推送！");
                            return;
                        }

                        var lstDeactivate = items.ToList();
                        lstDeactivate.ForEach(a => personInfo.items.Remove(a));
                    }
                }
                string templateId = EKBAPIClient.GetTemplateId(Constant.urlGetContractTemplateId); //获取合同模板ID
                if (string.IsNullOrEmpty(templateId)) return;                
                string venName = dao.GetVenName(cVenCode);
                long ddateTimestamp= Tool.ConvertToUnixTimestamp(DateTime.Parse(dDate));
                long effectTime = Tool.ConvertToUnixTimestamp(DateTime.Parse(cAuditDate));
                string currencyId = "";
                switch (cexch_name)
                {
                    case "人民币":
                        currencyId = EnumUtil.GetDesc(EnumCurrency.人民币);
                        break;
                    case "美元":
                        currencyId = EnumUtil.GetDesc(EnumCurrency.美元);
                        break;
                    case "欧元":
                        currencyId = EnumUtil.GetDesc(EnumCurrency.欧元);
                        break;
                    case "英镑":
                        currencyId = EnumUtil.GetDesc(EnumCurrency.英镑);
                        break;
                    default:
                        break;
                }

                string bustypeId = "";
                switch (cBusType)
                {
                    case "普通采购":
                        bustypeId = EnumUtil.GetDesc(EnumBusType.普通采购);
                        break;
                    case "固定资产":
                        bustypeId = EnumUtil.GetDesc(EnumBusType.固定资产);
                        break;
                    case "代管采购":
                        bustypeId = EnumUtil.GetDesc(EnumBusType.代管采购);
                        break;
                    case "受托代销":
                        bustypeId = EnumUtil.GetDesc(EnumBusType.受托代销);
                        break;
                    default:
                        bustypeId = EnumUtil.GetDesc(EnumBusType.无);
                        break;
                }

                MsgUtil.WriteFile("已获取枚举信息");

                Contract contract = new Contract();
                var form = new Contract.Form();
                //form.outerCode = ccode;
                form.specificationId = templateId;
                form.cMakeTime = Tool.ConvertToUnixTimestamp(DateTime.Now);
                form.contractCode = ccode;
                form.contractName = $"{venName}{ccode}";
                form.cItemCode = cItemCode;
                form.cItemName = cItemName;
                form.contractKind = EnumUtil.GetDesc(EnumCMKind.采购类合同);
                form.contractType = EnumUtil.GetDesc(EnumCMType.应付合同);
                form.contractMainCode = ccode;
                form.cVenName = venName;
                form.cVenContactMethod = cVenPerson;
                form.DateSign = ddateTimestamp;
                form.DateBegin = ddateTimestamp;
                form.DateEnd = ddateTimestamp;
                form.DateEffective = effectTime;
                form.currency = currencyId;
                form.exchangeRate = nflat;
                form.planDirection = EnumUtil.GetDesc(EnumPlanDirection.付);
                form.Department = personInfo.items[0].defaultDepartment;
                form.submitterId = personInfo.items[0].id;
                form.busType = bustypeId;
                form.iQua = QuaSum.ToString();
                form.sum = new Contract.Money() { standard=Sum.ToString() };
                form.natSum = new Contract.Money() { standard = NatSum.ToString() };
                form.memo = cMemo;

                contract.form = form;


                MsgUtil.WriteFile("参数封装完毕");

                if (EKBAPIClient.CreateContract(contract))
                {
                    dao.SetPOStatus(ccode);
                    MessageBox.Show("已成功创建易快报合同。","易快报合同创建",MessageBoxButtons.OK);
                    
                }

            }
            catch (Exception ex)
            {
                    errMsg = $"单据插件异常，错误原因：{MsgUtil.GetExceptionMsg(ex, ex.ToString())}";
                    MsgUtil.WriteFile($"{errMsg}");
                    MessageBox.Show(errMsg);
            }
        }
    }


    #region 枚举

    /// <summary>
    /// 合同类型
    /// </summary>
    public enum EnumCMType
    {

        [Description("ID01v4MX64Bkmj")]
        采购合同 = 1,
        [Description("ID01v4MVXjEhT9")]
        应付合同 = 2

    }

    /// <summary>
    /// 合同性质
    /// </summary>
    public enum EnumCMKind
    {
        [Description("ID01v4N3rdK7Pp")]
        采购类合同 = 1,
        [Description("ID01v4N60pbLbx")]
        应付类合同 = 2
    }

    /// <summary>
    /// 业务类型
    /// </summary>

    public enum EnumBusType
    {
        [Description("ID01v4OeAWT6ij")]
        无 = 1,
        [Description("ID01v4Ogibvmyz")]
        普通采购 = 2,
        [Description("ID01v4O9jaoSnl")]
        代管采购 = 3,
        [Description("ID01v4Ocm7IpD9")]
        受托代销 = 4,
        [Description("ID01v4Ogibz7nF")]
        固定资产 = 5

    }


    /// <summary>
    /// 收付款计划方向
    /// </summary>
    public enum EnumPlanDirection
    {
        [Description("ID01v4Nc0GaDIH")]
        付 = 1,
        [Description("ID01v4N60pslIj")]
        收 = 2

    }

    /// <summary>
    /// 币种
    /// </summary>
    public enum EnumCurrency
    {
        [Description("ID01v4N4388csn")]
        人民币 = 1,
        [Description("ID01v4N3VYtZ95")]
        美元 = 2,
        [Description("ID01v4N9p2rHZl")]
        欧元 = 3,
        [Description("ID01v4N8J2pc6P")]
        英镑 = 4
    } 

    #endregion
}
