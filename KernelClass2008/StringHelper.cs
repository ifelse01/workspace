using System;
using System.Collections.Generic;
using System.Web;
using System.Text.RegularExpressions;
using System.Text;
using System.IO;
using System.Security.Cryptography;
using System.Web.UI;
using System.Net;

namespace KernelClass
{
    public static class StringHelper
    {

        /// <summary>
        /// 产生随机数字字符串
        /// </summary>
        /// <returns></returns>
        public static string RandomStr(int Num)
        {
            int number;
            char code;
            string returnCode = String.Empty;

            Random random = new Random();

            for (int i = 0; i < Num; i++)
            {
                number = random.Next();
                code = (char)('0' + (char)(number % 10));
                returnCode += code.ToString();
            }
            return returnCode;
        }
        /// <summary>
        /// 获取querystring
        /// </summary>
        /// <param name="s">参数名</param>
        /// <returns>返回值</returns>
        public static string q(string s)
        {
            if (HttpContext.Current.Request.QueryString[s] != null && HttpContext.Current.Request.QueryString[s] != "")
            {
                return KernelClass.StringHelper.SafetyStr(HttpContext.Current.Request.QueryString[s].ToString());
            }
            return string.Empty;
        }

        /// <summary>
        /// 获取post得到的参数
        /// </summary>
        /// <param name="s">参数名</param>
        /// <returns>返回值</returns>
        public static string f(string s)
        {
            if (HttpContext.Current.Request.Form[s] != null && HttpContext.Current.Request.Form[s] != "")
            {
                return HttpContext.Current.Request.Form[s].ToString();
            }
            return string.Empty;
        }

        /// <summary>
        /// 返回非负整数，默认为t
        /// </summary>
        /// <param name="s">参数值</param>
        /// <returns>返回值</returns>
        public static int Str2Int(string s, int t)
        {
            return KernelClass.Validator.StrToInt(s, t);
        }

        /// <summary>
        /// 返回非负整数，默认为0
        /// </summary>
        /// <param name="s">参数值</param>
        /// <returns>返回值</returns>
        public static int Str2Int(string s)
        {
            return Str2Int(s, 0);
        }

        /// <summary>
        /// 返回非空字符串，默认为"0"
        /// </summary>
        /// <param name="s">参数值</param>
        /// <returns>返回值</returns>
        public static string Str2Str(string s)
        {
            return KernelClass.Validator.StrToInt(s, 0).ToString();
        }
        /// <summary>
        /// 字符串长度
        /// </summary>
        public static int GetStringLen(string str)
        {
            byte[] bs = System.Text.Encoding.Default.GetBytes(str);
            return bs.Length;
        }
        /// <summary>
        /// 字符串截断
        /// </summary>
        /// <param name="str"></param>
        /// <param name="Length">以汉字计算，比如Length为100表示取200个字符，100个汉字</param>
        /// <returns></returns>
        public static string GetCutString(string str, int Length)
        {
            Length *= 2;
            byte[] bs = System.Text.Encoding.Default.GetBytes(str);
            if (bs.Length <= Length)
            {
                return str;
            }
            else
            {
                return System.Text.Encoding.Default.GetString(bs, 0, Length);
            }
        }

        /// <summary>
        /// 返回字符串真实长度, 1个汉字长度为2
        /// </summary>
        /// <returns></returns>
        public static int GetStringLength(string str)
        {
            return Encoding.Default.GetBytes(str).Length;
        }

        /// <summary>
        /// 判断指定字符串在指定字符串数组中的位置
        /// </summary>
        /// <param name="strSearch">字符串</param>
        /// <param name="stringArray">字符串数组</param>
        /// <param name="caseInsensetive">是否不区分大小写, true为不区分, false为区分</param>
        /// <returns>字符串在指定字符串数组中的位置, 如不存在则返回-1</returns>
        public static int GetInArrayID(string strSearch, string[] stringArray, bool caseInsensetive)
        {
            for (int i = 0; i < stringArray.Length; i++)
            {
                if (caseInsensetive)
                {
                    if (strSearch.ToLower() == stringArray[i].ToLower())
                    {
                        return i;
                    }
                }
                else
                {
                    if (strSearch == stringArray[i])
                    {
                        return i;
                    }
                }

            }
            return -1;
        }

        /// <summary>
        /// 判断指定字符串在指定字符串数组中的位置
        /// </summary>
        /// <param name="strSearch">字符串</param>
        /// <param name="stringArray">字符串数组</param>
        /// <returns>字符串在指定字符串数组中的位置, 如不存在则返回-1</returns>		
        public static int GetInArrayID(string strSearch, string[] stringArray)
        {
            return GetInArrayID(strSearch, stringArray, true);
        }

        /// <summary>
        /// 判断指定字符串是否属于指定字符串数组中的一个元素
        /// </summary>
        /// <param name="strSearch">字符串</param>
        /// <param name="stringArray">字符串数组</param>
        /// <param name="caseInsensetive">是否不区分大小写, true为不区分, false为区分</param>
        /// <returns>判断结果</returns>
        public static bool InArray(string strSearch, string[] stringArray, bool caseInsensetive)
        {
            return GetInArrayID(strSearch, stringArray, caseInsensetive) >= 0;
        }

        /// <summary>
        /// 判断指定字符串是否属于指定字符串数组中的一个元素
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="stringarray">字符串数组</param>
        /// <returns>判断结果</returns>
        public static bool InArray(string str, string[] stringarray)
        {
            return InArray(str, stringarray, false);
        }

        /// <summary>
        /// 判断指定字符串是否属于指定字符串数组中的一个元素
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="stringarray">内部以逗号分割单词的字符串</param>
        /// <returns>判断结果</returns>
        public static bool InArray(string str, string stringarray)
        {
            return InArray(str, SplitString(stringarray, ","), false);
        }
        
        /// <summary>
        /// 判断指定字符串是否属于指定字符串数组中的一个元素
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="stringarray">内部以逗号分割单词的字符串</param>
        /// <param name="strsplit">分割字符串</param>
        /// <returns>判断结果</returns>
        public static bool InArray(string str, string stringarray, string strsplit)
        {
            return InArray(str, SplitString(stringarray, strsplit), false);
        }
        
        /// <summary>
        /// 判断指定字符串是否属于指定字符串数组中的一个元素
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="stringarray">内部以逗号分割单词的字符串</param>
        /// <param name="strsplit">分割字符串</param>
        /// <param name="caseInsensetive">是否不区分大小写, true为不区分, false为区分</param>
        /// <returns>判断结果</returns>
        public static bool InArray(string str, string stringarray, string strsplit, bool caseInsensetive)
        {
            return InArray(str, SplitString(stringarray, strsplit), caseInsensetive);
        }

        /// <summary>
        /// 分割字符串
        /// </summary>
        public static string[] SplitString(string strContent, string strSplit)
        {
            if (strContent.IndexOf(strSplit) < 0)
            {
                string[] tmp = { strContent };
                return tmp;
            }
            return Regex.Split(strContent, Regex.Escape(strSplit), RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// 分割字符串
        /// </summary>
        /// <returns></returns>
        public static string[] SplitString(string strContent, string strSplit, int p_3)
        {
            string[] result = new string[p_3];

            string[] splited = SplitString(strContent, strSplit);

            for (int i = 0; i < p_3; i++)
            {
                if (i < splited.Length)
                    result[i] = splited[i];
                else
                    result[i] = string.Empty;
            }

            return result;
        }

        /// <summary>
        /// 从字符串的指定位置截取指定长度的子字符串
        /// </summary>
        /// <param name="str">原字符串</param>
        /// <param name="startIndex">子字符串的起始位置</param>
        /// <param name="length">子字符串的长度</param>
        /// <returns>子字符串</returns>
        public static string CutString(string str, int startIndex, int length)
        {
            if (startIndex >= 0)
            {
                if (length < 0)
                {
                    length = length * -1;
                    if (startIndex - length < 0)
                    {
                        length = startIndex;
                        startIndex = 0;
                    }
                    else
                    {
                        startIndex = startIndex - length;
                    }
                }


                if (startIndex > str.Length)
                {
                    return "";
                }


            }
            else
            {
                if (length < 0)
                {
                    return "";
                }
                else
                {
                    if (length + startIndex > 0)
                    {
                        length = length + startIndex;
                        startIndex = 0;
                    }
                    else
                    {
                        return "";
                    }
                }
            }

            if (str.Length - startIndex < length)
            {
                length = str.Length - startIndex;
            }

            return str.Substring(startIndex, length);
        }


        /// <summary>
        /// 从字符串的指定位置开始截取到字符串结尾的了符串
        /// </summary>
        /// <param name="str">原字符串</param>
        /// <param name="startIndex">子字符串的起始位置</param>
        /// <returns>子字符串</returns>
        public static string CutString(string str, int startIndex)
        {
            return CutString(str, startIndex, str.Length);
        }

        /// <summary>
        /// 获得当前绝对路径
        /// </summary>
        /// <param name="strPath">指定的路径</param>
        /// <returns>绝对路径</returns>
        public static string GetMapPath(string strPath)
        {
            if (HttpContext.Current != null)
            {
                return HttpContext.Current.Server.MapPath(strPath);
            }
            else //非web程序引用
            {
                return System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, strPath);
            }
        }
              
        /// <summary>
        /// 判断文件名是否为浏览器可以直接显示的图片文件名
        /// </summary>
        /// <param name="filename">文件名</param>
        /// <returns>是否可以直接显示</returns>
        public static bool IsImgFilename(string filename)
        {
            filename = filename.Trim();
            if (filename.EndsWith(".") || filename.IndexOf(".") == -1)
            {
                return false;
            }
            string extname = filename.Substring(filename.LastIndexOf(".") + 1).ToLower();
            return (extname == "jpg" || extname == "jpeg" || extname == "png" || extname == "bmp" || extname == "gif");
        }

        /// <summary>
        /// MD5函数(加密方法)
        /// </summary>
        /// <param name="str">原始字符串</param>
        /// <returns>MD5结果</returns>
        public static string MD5(string str)
        {
            byte[] b = Encoding.Default.GetBytes(str);
            b = new MD5CryptoServiceProvider().ComputeHash(b);
            string ret = "";
            for (int i = 0; i < b.Length; i++)
                ret += b[i].ToString("x").PadLeft(2, '0');
            return ret;
        }
        public static string getMd5Hash(string input)
        {
            // Create a new instance of the MD5CryptoServiceProvider object.
            System.Security.Cryptography.MD5 md5Hasher = System.Security.Cryptography.MD5.Create();

            // Convert the input string to a byte array and compute the hash.
            byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }

        /// <summary>
        /// SHA256函数(加密方法，该加密算法比MD5更强大)
        /// </summary>
        /// /// <param name="str">原始字符串</param>
        /// <returns>SHA256结果</returns>
        public static string SHA256(string str)
        {
            byte[] SHA256Data = Encoding.UTF8.GetBytes(str);
            SHA256Managed Sha256 = new SHA256Managed();
            byte[] Result = Sha256.ComputeHash(SHA256Data);
            return Convert.ToBase64String(Result);  //返回长度为44字节的字符串
        }

        /// <summary>
        /// 字符串如果操过指定长度则将超出的部分用指定字符串代替
        /// </summary>
        /// <param name="p_SrcString">要检查的字符串</param>
        /// <param name="p_Length">指定长度</param>
        /// <param name="p_TailString">用于替换的字符串</param>
        /// <returns>截取后的字符串</returns>
        public static string GetSubString(string p_SrcString, int p_Length, string p_TailString)
        {
            return GetSubString(p_SrcString, 0, p_Length, p_TailString);
        }


        /// <summary>
        /// 取指定长度的字符串
        /// </summary>
        /// <param name="p_SrcString">要检查的字符串</param>
        /// <param name="p_StartIndex">起始位置</param>
        /// <param name="p_Length">指定长度</param>
        /// <param name="p_TailString">用于替换的字符串</param>
        /// <returns>截取后的字符串</returns>
        public static string GetSubString(string p_SrcString, int p_StartIndex, int p_Length, string p_TailString)
        {


            string myResult = p_SrcString;

            //当是日文或韩文时(注:中文的范围:\u4e00 - \u9fa5, 日文在\u0800 - \u4e00, 韩文为\xAC00-\xD7A3)
            if (System.Text.RegularExpressions.Regex.IsMatch(p_SrcString, "[\u0800-\u4e00]+") ||
                System.Text.RegularExpressions.Regex.IsMatch(p_SrcString, "[\xAC00-\xD7A3]+"))
            {
                //当截取的起始位置超出字段串长度时
                if (p_StartIndex >= p_SrcString.Length)
                {
                    return "";
                }
                else
                {
                    return p_SrcString.Substring(p_StartIndex,
                                                   ((p_Length + p_StartIndex) > p_SrcString.Length) ? (p_SrcString.Length - p_StartIndex) : p_Length);
                }
            }


            if (p_Length >= 0)
            {
                byte[] bsSrcString = Encoding.Default.GetBytes(p_SrcString);

                //当字符串长度大于起始位置
                if (bsSrcString.Length > p_StartIndex)
                {
                    int p_EndIndex = bsSrcString.Length;

                    //当要截取的长度在字符串的有效长度范围内
                    if (bsSrcString.Length > (p_StartIndex + p_Length))
                    {
                        p_EndIndex = p_Length + p_StartIndex;
                    }
                    else
                    {   //当不在有效范围内时,只取到字符串的结尾

                        p_Length = bsSrcString.Length - p_StartIndex;
                        p_TailString = "";
                    }



                    int nRealLength = p_Length;
                    int[] anResultFlag = new int[p_Length];
                    byte[] bsResult = null;

                    int nFlag = 0;
                    for (int i = p_StartIndex; i < p_EndIndex; i++)
                    {

                        if (bsSrcString[i] > 127)
                        {
                            nFlag++;
                            if (nFlag == 3)
                            {
                                nFlag = 1;
                            }
                        }
                        else
                        {
                            nFlag = 0;
                        }

                        anResultFlag[i] = nFlag;
                    }

                    if ((bsSrcString[p_EndIndex - 1] > 127) && (anResultFlag[p_Length - 1] == 1))
                    {
                        nRealLength = p_Length + 1;
                    }

                    bsResult = new byte[nRealLength];

                    Array.Copy(bsSrcString, p_StartIndex, bsResult, 0, nRealLength);

                    myResult = Encoding.Default.GetString(bsResult);

                    myResult = myResult + p_TailString;
                }
            }

            return myResult;
        }

        /// <summary>
        /// 自定义的替换字符串函数
        /// </summary>
        public static string ReplaceString(string SourceString, string SearchString, string ReplaceString, bool IsCaseInsensetive)
        {
            return Regex.Replace(SourceString, Regex.Escape(SearchString), ReplaceString, IsCaseInsensetive ? RegexOptions.IgnoreCase : RegexOptions.None);
        }

        /// <summary>
        /// 生成指定数量的html空格符号
        /// </summary>
        public static string Spaces(int nSpaces)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < nSpaces; i++)
            {
                sb.Append(" &nbsp;&nbsp;");
            }
            return sb.ToString();
        }

        /// <summary>
        /// 获取Email主机名(如:abc@163.com的主机是163.com)
        /// </summary>
        /// <param name="strEmail"></param>
        /// <returns></returns>
        public static string GetEmailHostName(string strEmail)
        {
            if (strEmail.IndexOf("@") < 0)
            {
                return "";
            }
            return strEmail.Substring(strEmail.LastIndexOf("@")).ToLower();
        }


        /// <summary>
        /// 返回URL中结尾的参数名
        /// </summary>		
        public static string GetFilename(string url)
        {
            if (url == null)
            {
                return "";
            }
            string[] strs1 = url.Split(new char[] { '/' });
            return strs1[strs1.Length - 1].Split(new char[] { '?' })[0];
        }


        /// <summary>
        /// 返回 URL 字符串的编码结果
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns>编码结果</returns>
        public static string UrlEncode(string str)
        {
            return HttpUtility.UrlEncode(str);
        }

        /// <summary>
        /// 根据阿拉伯数字返回月份的名称(可更改为某种语言)
        /// </summary>	
        public static string[] Monthes
        {
            get
            {
                return new string[] { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };
            }
        }

        
        /// <summary>
        /// 返回 HTML 字符串的编码结果
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns>编码结果</returns>
        public static string HtmlEncode(string str)
        {
            return HttpUtility.HtmlEncode(str);
        }

        /// <summary>
        /// 返回 HTML 字符串的解码结果
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns>解码结果</returns>
        public static string HtmlDecode(string str)
        {
            return HttpUtility.HtmlDecode(str);
        }


        /// <summary>
        /// 返回 URL 字符串的解码结果
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns>解码结果</returns>
        public static string UrlDecode(string str)
        {
            return HttpUtility.UrlDecode(str);
        }

        /// <summary>
        /// 备份文件
        /// </summary>
        /// <param name="sourceFileName">源文件名</param>
        /// <param name="destFileName">目标文件名</param>
        /// <param name="overwrite">当目标文件存在时是否覆盖</param>
        /// <returns>操作是否成功</returns>
        public static bool BackupFile(string sourceFileName, string destFileName, bool overwrite)
        {
            if (!System.IO.File.Exists(sourceFileName))
            {
                throw new FileNotFoundException(sourceFileName + "文件不存在！");
            }
            if (!overwrite && System.IO.File.Exists(destFileName))
            {
                return false;
            }
            try
            {
                System.IO.File.Copy(sourceFileName, destFileName, true);
                return true;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        /// <summary>
        /// 备份文件,当目标文件存在时覆盖
        /// </summary>
        /// <param name="sourceFileName">源文件名</param>
        /// <param name="destFileName">目标文件名</param>
        /// <returns>操作是否成功</returns>
        public static bool BackupFile(string sourceFileName, string destFileName)
        {
            return BackupFile(sourceFileName, destFileName, true);
        }

        /// <summary>
        /// 拷贝文件
        /// </summary>
        /// <param name="backupFileName">备份文件名</param>
        /// <param name="targetFileName">要拷贝的文件名</param>
        /// <param name="backupTargetFileName">要拷贝文件再次备份的名称,如果为null,则不再备份拷贝文件</param>
        /// <returns>操作是否成功</returns>
        public static bool RestoreFile(string backupFileName, string targetFileName, string backupTargetFileName)
        {
            try
            {
                if (!System.IO.File.Exists(backupFileName))
                {
                    throw new FileNotFoundException(backupFileName + "文件不存在！");
                }
                if (backupTargetFileName != null)
                {
                    if (!System.IO.File.Exists(targetFileName))
                    {
                        throw new FileNotFoundException(targetFileName + "文件不存在！无法备份此文件！");
                    }
                    else
                    {
                        System.IO.File.Copy(targetFileName, backupTargetFileName, true);
                    }
                }
                System.IO.File.Delete(targetFileName);
                System.IO.File.Copy(backupFileName, targetFileName);
            }
            catch (Exception e)
            {
                throw e;
            }
            return true;
        }

        /// <summary>
        /// 拷贝文件
        /// </summary>
        /// <param name="backupFileName">备份文件名</param>
        /// <param name="targetFileName">要拷贝的文件名</param>
        /// <returns>操作结果</returns>
        public static bool RestoreFile(string backupFileName, string targetFileName)
        {
            return RestoreFile(backupFileName, targetFileName, null);
        }


        /// <summary>
        /// 过滤所有特殊特号
        /// </summary>
        /// <param name="theString"></param>
        /// <returns></returns>
        public static string FilterSymbol(string theString)
        {
            string[] aryReg = { "'", "\"", "\r", "\n", "<", ">", "%", "?", ",", ".", "=", "-", "_", ";", "|", "[", "]", "&", "/" };
            for (int i = 0; i < aryReg.Length; i++)
            {
                theString = theString.Replace(aryReg[i], string.Empty);
            }
            return theString;
        }
        /// <summary>
        /// 过滤一般特殊特号,如单引、双引和回车和分号等
        /// </summary>
        /// <param name="theString"></param>
        /// <returns></returns>
        public static string SafetyStr(string theString)
        {
            string[] aryReg = { "'", ";", "\"", "\r", "\n" };
            for (int i = 0; i < aryReg.Length; i++)
            {
                theString = theString.Replace(aryReg[i], string.Empty);
            }
            return theString;
        }
                
	    /// <summary>
	    ///  Encode the Text string  --  like Dimension ASP
	    /// </summary>
	    /// <param name="Text"></param>
	    /// 
	    public static string DM_Encode64(string Message)
	    {
	        char[] Base64Code = new char[]
	              {
	                'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N',
	                  'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', 'a', 'b',
	                  'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p',
	                  'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', '0', '1', '2', '3',
	                  '4', '5', '6', '7', '8', '9', '-', '_', '='
	              };
	        byte empty = (byte)0;
	        System.Collections.ArrayList byteMessage = new
	          System.Collections.ArrayList(System.Text.Encoding.Default.GetBytes
	          (Message));
	        System.Text.StringBuilder outmessage;
	        int messageLen = byteMessage.Count;
	        int page = messageLen / 3;
	        int use = 0;
	        if ((use = messageLen % 3) > 0)
	        {
	            for (int i = 0; i < 3 - use; i++)
	                byteMessage.Add(empty);
	            page++;
	        }
	        outmessage = new System.Text.StringBuilder(page * 4);
	        for (int i = 0; i < page; i++)
	        {
	            byte[] instr = new byte[3];
	            instr[0] = (byte)byteMessage[i * 3];
	            instr[1] = (byte)byteMessage[i * 3 + 1];
	            instr[2] = (byte)byteMessage[i * 3 + 2];
	            int[] outstr = new int[4];
	            outstr[0] = instr[0] >> 2;
	            outmessage.Append(Base64Code[outstr[0]]);
	
	            outstr[1] = ((instr[0] & 0x03) << 4) ^ (instr[1] >> 4);
	            outmessage.Append(Base64Code[outstr[1]]);
	
	            if (!instr[1].Equals(empty))
	            {
	                outstr[2] = ((instr[1] & 0x0f) << 2) ^ (instr[2] >> 6);
	                outmessage.Append(Base64Code[outstr[2]]);
	            }
	
	
	            if (!instr[2].Equals(empty))
	            {
	                outstr[3] = (instr[2] & 0x3f);
	
	                outmessage.Append(Base64Code[outstr[3]]);
	            }
	        }
	        return outmessage.ToString();
	    }
	
	    /// <summary>
	    ///  Decode the Text string  --  like Dimension ASP
	    /// </summary>
	    /// <param name="Text"></param>
	    /// 
        public static string DM_Decode64(string Message)
	    {
	        string Base64Code = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789-_=";
	        int page = Message.Length / 4;
	        System.Collections.ArrayList outMessage = new System.Collections.ArrayList(page * 3);
	        char[] message = Message.ToCharArray();
            int length = message.Length;
	        for (int i = 0; i <= page; i++)
	        {
	            byte[] instr = new byte[4];
	            if (length > (i * 4))
	                instr[0] = (byte)Base64Code.IndexOf(message[i * 4]);
	            if (length > (i * 4 + 1))
	                instr[1] = (byte)Base64Code.IndexOf(message[i * 4 + 1]);
	            if (length > (i * 4 + 2))
	                instr[2] = (byte)Base64Code.IndexOf(message[i * 4 + 2]);
	            if (length > (i * 4 + 3))
	                instr[3] = (byte)Base64Code.IndexOf(message[i * 4 + 3]);
	            byte[] outstr = new byte[3];
	            outstr[0] = (byte)((instr[0] << 2) ^ ((instr[1] & 0x30) >> 4));
	            if (instr[2] != 64)
	            {
	                outstr[1] = (byte)((instr[1] << 4) ^ ((instr[2] & 0x3c) >> 2));
	            }
	            else
	            {
	                outstr[2] = 0;
	            }
	            if (instr[3] != 64)
	            {
	                outstr[2] = (byte)((instr[2] << 6) ^ instr[3]);
	            }
	            else
	            {
	                outstr[2] = 0;
	            }
	            outMessage.Add(outstr[0]);
	            if (outstr[1] != 0)
	                outMessage.Add(outstr[1]);
	            if (outstr[2] != 0)
	                outMessage.Add(outstr[2]);
	        }
	        byte[] outbyte = (byte[])outMessage.ToArray(Type.GetType("System.Byte"));
	        return System.Text.Encoding.Default.GetString(outbyte);
	      
	    }

        /// <summary>
        /// 获得当前页面客户端的IP
        /// </summary>
        /// <returns>当前页面客户端的IP</returns>
        public static string GetIP()
        {


            string result = String.Empty;

            result = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (null == result || result == String.Empty)
            {
                result = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }

            if (null == result || result == String.Empty)
            {
                result = HttpContext.Current.Request.UserHostAddress;
            }

            if (null == result || result == String.Empty || ! Validator.IsIP(result))
            {
                return "0.0.0.0";
            }

            return result;

        }

        /// <summary>
        /// 将IP地址转为整数形式
        /// </summary>
        /// <returns>整数</returns>
        public static long IP2Long(IPAddress ip)
        {
            int x = 3;
            long o = 0;
            foreach (byte f in ip.GetAddressBytes())
            {
                o += (long)f << 8 * x--;
            }
            return o;
        }
        /// <summary>
        /// 将整数转为IP地址
        /// </summary>
        /// <returns>IP地址</returns>
        public static IPAddress Long2IP(long l)
        {
            byte[] b = new byte[4];
            for (int i = 0; i < 4; i++)
            {
                b[3 - i] = (byte)(l >> 8 * i & 255);
            }
            return new IPAddress(b);
        }

        /// <summary>
        /// 格式化IP
        /// </summary>
        public static string FormatIP(string ipStr)
        {
            string[] temp = ipStr.Split('.');
            string format = "";
            for (int i = 0; i < temp.Length; i++)
            {
                if (temp[i].Length < 3) temp[i] = Convert.ToString("000" + temp[i]).Substring(Convert.ToString("000" + temp[i]).Length - 3, 3);
                format += temp[i].ToString();
            }
            return format;
        }

        /// <summary>
        /// 来源地址
        /// </summary>
        public static string GetRefererUrl
        {
            get
            {
                if (HttpContext.Current.Request.ServerVariables["Http_Referer"] == null)
                    return "";
                else
                    return HttpContext.Current.Request.ServerVariables["Http_Referer"].ToString();
            }
        }

        /// <summary>
        /// 当前地址
        /// </summary>
        public static string GetCurrentUrl
        {
            get
            {
                string strUrl;
                strUrl = HttpContext.Current.Request.ServerVariables["Url"];
                if (HttpContext.Current.Request.QueryString.Count == 0) //如果无参数
                    return strUrl;
                else
                    return strUrl + "?" + HttpContext.Current.Request.ServerVariables["Query_String"];
            }

        }
        /// <summary>
        /// 不区分大小写的替换
        /// </summary>
        /// <param name="original">原字符串</param>
        /// <param name="pattern">需替换字符</param>
        /// <param name="replacement">被替换内容</param>
        /// <returns></returns>
        public static string ReplaceEx(string original, string pattern, string replacement)
        {
            int count, position0, position1;
            count = position0 = position1 = 0;
            string upperString = original.ToUpper();
            string upperPattern = pattern.ToUpper();
            int inc = (original.Length / pattern.Length) * (replacement.Length - pattern.Length);
            char[] chars = new char[original.Length + Math.Max(0, inc)];
            while ((position1 = upperString.IndexOf(upperPattern, position0)) != -1)
            {
                for (int i = position0; i < position1; ++i) chars[count++] = original[i];
                for (int i = 0; i < replacement.Length; ++i) chars[count++] = replacement[i];
                position0 = position1 + pattern.Length;
            }
            if (position0 == 0) return original;
            for (int i = position0; i < original.Length; ++i) chars[count++] = original[i];
            return new string(chars, 0, count);
        }
        ///<summary>   
        ///去除HTML标记   
        ///</summary>   
        ///<param name="NoHTML">包括HTML的源码</param>   
        ///<returns>已经去除后的文字</returns>   
        public static string NoHTML(string Htmlstring)
        {
            //Regex myReg = new Regex(@"(\<.[^\<]*\>)", RegexOptions.IgnoreCase);
            //Htmlstring = myReg.Replace(Htmlstring, "");
            //myReg = new Regex(@"(\<\/[^\<]*\>)", RegexOptions.IgnoreCase);
            //Htmlstring = myReg.Replace(Htmlstring, "");
            //return Htmlstring;

            //删除脚本
            Htmlstring = Regex.Replace(Htmlstring, @"<script[^>]*?>.*?</script>", "", RegexOptions.IgnoreCase);
            //删除HTML
            Htmlstring = Regex.Replace(Htmlstring, @"<(.[^>]*)>", "", RegexOptions.IgnoreCase);
            //Htmlstring = Regex.Replace(Htmlstring, @"([\r\n])[\s]+", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"-->", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"<!--.*", "", RegexOptions.IgnoreCase);

            Htmlstring = Regex.Replace(Htmlstring, @"&(quot|#34);", "\"", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&ldquo;", "“", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&rdquo;", "”", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(amp|#38);", "&", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(lt|#60);", "<", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(gt|#62);", ">", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(nbsp|#160);", "   ", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(iexcl|#161);", "\xa1", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(cent|#162);", "\xa2", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(pound|#163);", "\xa3", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(copy|#169);", "\xa9", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&#(\d+);", "", RegexOptions.IgnoreCase);

            Htmlstring = Htmlstring.Replace("<", "&lt;");
            Htmlstring = Htmlstring.Replace(">", "&gt;");
            return Htmlstring;
        }
        /// <summary>        
        /// 格式化显示时间为几个月,几天前,几小时前,几分钟前,或几秒前        
        /// </summary>        
        /// <param name="dt">要格式化显示的时间</param>        
        /// <returns>几个月,几天前,几小时前,几分钟前,或几秒前</returns>        
        public static string DateStringFromNow(DateTime dt)
        {
            TimeSpan span = DateTime.Now - dt;
            if (span.TotalDays > 60) { return dt.ToShortDateString(); }
            else if (span.TotalDays > 30) { return "1个月前"; }
            else if (span.TotalDays > 14) { return "2周前"; }
            else if (span.TotalDays > 7) { return "1周前"; }
            else if (span.TotalDays > 1) { return string.Format("{0}天前", (int)Math.Floor(span.TotalDays)); }
            else if (span.TotalHours > 1) { return string.Format("{0}小时前", (int)Math.Floor(span.TotalHours)); }
            else if (span.TotalMinutes > 1) { return string.Format("{0}分钟前", (int)Math.Floor(span.TotalMinutes)); }
            else if (span.TotalSeconds >= 1) { return string.Format("{0}秒前", (int)Math.Floor(span.TotalSeconds)); }
            else { return "1秒前"; }
        }
    }
}