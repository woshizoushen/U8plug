using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace UFIDA.U8.Portal.ZT.VouchPlugIns.Tools
{
    public class MsgUtil
    {

        public static List<string> ErrorMsg;

        /// <summary> 
        /// 生成自定义异常消息 
        /// </summary> 
        /// <param name="ex">异常对象</param> 
        /// <param name="backStr">备用异常消息：当ex为null时有效</param> 
        /// <returns>异常字符串文本</returns> 
        public static string GetExceptionMsg(Exception ex, string backStr)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("【出现时间】：" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"));
            if (ex != null)
            {
                sb.AppendLine("【异常类型】：" + ex.GetType().Name);
                sb.AppendLine("【异常信息】：" + ex.Message);
                sb.AppendLine("【堆栈调用】：" + ex.StackTrace);
                sb.AppendLine("【异常方法】：" + ex.TargetSite);
            }
            else
            {
                sb.AppendLine("【未处理异常】：" + backStr);
            }

            return sb.ToString();
        }




        /// <summary>
        /// 生成错误日志
        /// </summary>
        /// <param name="FileContent"></param>
        /// <returns></returns>
        public static bool WriteFile(string FileContent)
        {
            StreamWriter streamWriter = null;
            string fileName = string.Format("{0}.txt", DateTime.Now.ToString("yyyyMMdd"));
            string path = Directory.GetCurrentDirectory() + "\\Logs\\";
            string filePath = path + fileName;
            try
            {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                if (File.Exists(filePath))
                {
                    streamWriter = new StreamWriter(filePath, true, Encoding.GetEncoding("UTF-8"));
                }
                else
                {
                    streamWriter = new StreamWriter(filePath, false, Encoding.GetEncoding("UTF-8"));
                }


                string str2 = DateTime.Now.ToString("【yyyy-MM-dd HH:mm:sss】 ");
                if (!String.IsNullOrEmpty(FileContent) && !String.IsNullOrEmpty(FileContent))
                {
                    streamWriter.WriteLine(str2 + FileContent);
                }
                else
                {
                    streamWriter.WriteLine();
                }
            }
            catch (IOException ex)
            {
                string message = ex.Message;
                return false;
            }
            finally
            {
                if (streamWriter != null)
                {
                    streamWriter.Close();
                }
            }
            return true;
        }
    }
}
