using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Reflection;
using System.IO;
using System.Data;
using System.Web;
using System.Data.Odbc;
using System.Text.RegularExpressions;
using System.Collections;

namespace KernelClass
{
    public class CommonHelper
    {
        public static Dictionary<string, List<string>> GetArgs(string[] args)
        {
            char SeparatorChar = ':';
            List<string> listEmptyKeyList = new List<string>();
            Dictionary<string, List<string>> dic = new Dictionary<string, List<string>>();
            foreach (var s in args)
            {
                if (s.Contains(SeparatorChar))
                {
                    string[] arrStr = s.Split(SeparatorChar);
                    if (dic.ContainsKey(arrStr[0]))
                    {
                        dic[arrStr[0]].Add(arrStr[1]);
                    }
                    else
                    {
                        dic.Add(arrStr[0], new List<string>{arrStr[1]});
                    }
                }
                else
                {
                    listEmptyKeyList.Add(s);
                }
            }
            if(listEmptyKeyList.Count > 0)
            {
                dic.Add("empty", listEmptyKeyList);
            }
            return dic;
        }
    }

}
