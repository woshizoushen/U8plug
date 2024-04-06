using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UFIDA.U8.Portal.ZT.VouchPlugIns
{

    public interface ICommand
    {
        /// <summary>
        /// 按钮类型为default的接口
        /// </summary>
        void RunCommand(object objLogin, object objForm, object objVoucher, string sKey, object VarentValue, string other);
        /// <summary>
        /// 按钮的初始化接口
        /// </summary>
        void Init(object objLogin, object objForm, object objVoucher, object msbar);
        /// <summary>
        /// 按钮类型为system的接口
        /// </summary>
        void BeforeRunSysCommand(object objLogin, object objForm, object objVoucher, string sKey, object VarentValue, ref bool Cancel, string other);
        /// <summary>
        /// 按钮类型为system的接口，执行系统功能后
        /// </summary>
        void AfterRunSysCommand(object objLogin, object objForm, object objVoucher, string sKey, object VarentValue, ref bool Cancel, string other);
    }
}
