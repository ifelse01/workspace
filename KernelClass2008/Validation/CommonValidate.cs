using System;
using System.Collections.Generic;
using System.Web;
using System.Text;
using System.Text.RegularExpressions;

namespace KernelClass
{
    public static class CommonValidate
    {
        private readonly static Regex RegNumber = new Regex("^[0-9]+$");
        private readonly static Regex RegNumberSign = new Regex("^[+-]?[0-9]+$");
        private readonly static Regex RegDecimal = new Regex("^[0-9]+[.]?[0-9]+$");              //等价于^[+-]?\d+[.]?\d+$   
        private readonly static Regex RegDecimalSign = new Regex("^[+-]?[0-9]+[.]?[0-9]+$");     //w 英文字母或数字的字符串，和 [a-zA-Z0-9] 语法一样    
        private readonly static Regex RegEmail = new Regex("^[\\w-]+@[\\w-]+\\.([a-zA-Z]{2,3}|[0-9]{1,3})(\\]?)$");     //^[\\w-]+@[\\w-]+\\.(com|net|org|edu|mil|tv|biz|info)$
        private readonly static Regex RegEmailMul = new Regex(".+\\@(\\[?)[a-zA-Z0-9\\-\\.]+\\.([a-zA-Z]{2,3}|[0-9]{1,3})(\\]?)");

        #region 数字字符串检查
        /// <summary>   
        /// 是否数字字符串   
        /// </summary>   
        /// <param name="inputData">输入字符串</param>   
        /// <returns></returns>   
        public static bool IsNumber(string inputData)
        {
            Match m = RegNumber.Match(inputData);
            return m.Success;
        }

        /// <summary>   
        /// 是否数字字符串 可带正负号   
        /// </summary>   
        /// <param name="inputData">输入字符串</param>   
        /// <returns></returns>   
        public static bool IsNumberSign(string inputData)
        {
            Match m = RegNumberSign.Match(inputData);
            return m.Success;
        }

        /// <summary>   
        /// 是否是浮点数   
        /// </summary>   
        /// <param name="inputData">输入字符串</param>   
        /// <returns></returns>   
        public static bool IsDecimal(string inputData)
        {
            Match m = RegDecimal.Match(inputData);
            return m.Success;
        }

        /// <summary>   
        /// 是否是浮点数 可带正负号   
        /// </summary>   
        /// <param name="inputData">输入字符串</param>   
        /// <returns></returns>   
        public static bool IsDecimalSign(string inputData)
        {
            Match m = RegDecimalSign.Match(inputData);
            return m.Success;
        }
        #endregion

        #region 邮件地址
        /// <summary>   
        /// 是否是邮件地址
        /// </summary>   
        /// <param name="inputData">输入字符串</param>   
        /// <returns></returns>   
        public static bool IsEmail(string inputData)
        {
            Match m = RegEmail.Match(inputData);
            return m.Success;
        }

        /// <summary>   
        /// 是否是多个邮件地址
        /// </summary>   
        /// <param name="inputData">输入字符串</param>   
        /// <returns></returns>   
        public static bool IsEmailMul(string inputData)
        {
            Match m = RegEmailMul.Match(inputData);
            return m.Success;
        }
        #endregion
    }
}