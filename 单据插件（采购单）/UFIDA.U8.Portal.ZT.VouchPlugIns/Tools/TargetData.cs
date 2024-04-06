using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using UFIDA.U8.UAP.Plus.Common.DB;
using UFIDA.U8.UAP.Plus.Common.Util;

namespace UFIDA.U8.Portal.ZT.VouchPlugIns.Tools
{
    public class TargetData: BaseDAO
    {
        public string accId;


        #region Members

        /// <summary>
        /// 数据库连接串
        /// </summary>
        private string _connectionString;
        /// <summary>
        /// 数据库连接操作层
        /// </summary>
        private DBHelper _db;



        #endregion


        #region Properties

        /// <summary>
        /// 数据库连接操作层
        /// </summary>
        public override DBHelper DB
        {
            get
            {
                if (_v.IsNull(this._db))
                {
                    this._db = new DBHelper(_connectionString);
                }
                return this._db;
            }
        }

        #endregion



        #region Constructors
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="connectionString">数据库连接字符串</param>
        public TargetData(string connectionString)
        {
            this._connectionString = connectionString;
        }
        #endregion



        #region Methods


        public string GetVenName(string venCode)
        {
            string sql = $"SELECT cVenName FROM dbo.Vendor WHERE cVenCode='{venCode}'";
            string venName = "";
            try
            {
                venName = DB.ExecuteString(sql);
                if (DB.Logger.ErrorMessages.Count > 0) throw new Exception(DB.Logger.ErrorMessages.LastMessage);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Methods Error > GetVenCode > {ex.Message}");
                DB.Logger.ErrorMessages.Clear();
            }

            return venName;
        }

        /// <summary>
        /// 获取采购订单同步状态
        /// </summary>
        /// <param name="ccode"></param>
        /// <returns></returns>
        public string GetPOStatus(string ccode) 
        {
            string sql = $"SELECT ISNULL(cDefine3,'') FROM dbo.PO_Pomain WHERE cPOID='{ccode}'";
            string status = "";
            try
            {
                status = DB.ExecuteString(sql);
                if (DB.Logger.ErrorMessages.Count > 0) throw new Exception(DB.Logger.ErrorMessages.LastMessage);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Methods Error > GetPOStatus > {ex.Message}");
                DB.Logger.ErrorMessages.Clear();
            }

            return status;
        }


        public bool SetPOStatus(string ccode) 
        {
            string sql = $"UPDATE dbo.PO_Pomain SET cDefine3='已创建' WHERE cPOID='{ccode}'";
            int result = 0;
            try
            {
                result = DB.Execute(sql);
                if (result > 0) return true;
                if (DB.Logger.ErrorMessages.Count > 0) throw new Exception(DB.Logger.ErrorMessages.LastMessage);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Methods Error > SetPOStatus > {ex.Message}");
                DB.Logger.ErrorMessages.Clear();
            }

            return false;
        }


        public string GetJobNum(string person) 
        {
            string sql = $"SELECT JobNumber FROM dbo.hr_hi_person WHERE cPsn_Num='{person}'";
            string jobnum = "";
            try
            {
                jobnum = DB.ExecuteString(sql);
                if (DB.Logger.ErrorMessages.Count > 0) throw new Exception(DB.Logger.ErrorMessages.LastMessage);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Methods Error > GetJobNum > {ex.Message}");
                DB.Logger.ErrorMessages.Clear();
            }

            return jobnum;
        }

        #endregion
    }
}
