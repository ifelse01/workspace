
using System;
using System.DirectoryServices;
using System.Collections;
using System.Text.RegularExpressions;
using System.Text;
using Microsoft.Web.Administration;
using System.Diagnostics;
using System.IO;

namespace KernelClass
{
    /// <summary> 
    /// 这个类是静态类。用来实现管理IIS的基本操作。 
    /// 管理IIS有两种方式，一是ADSI，一是WMI。由于系统限制的原因，只好选择使用ADSI实现功能。 
    /// 这是一个遗憾。只有等到只有使用IIS 6的时候，才有可能使用WMI来管理系统 
    /// 不过有一个问题就是，我现在也觉得这样的一个方法在本地执行会比较的好。最好不要远程执行。 
    /// 因为那样需要占用相当数量的带宽，即使要远程执行，也是推荐在同一个网段里面执行 
    /// </summary> 
    public class IISAdminHelper
    {
        #region UserName,Password,HostName的定义
        public static string HostName
        {
            get
            {
                return hostName;
            }
            set
            {
                hostName = value;
            }
        }


        public static string UserName
        {
            get
            {
                return userName;
            }
            set
            {
                userName = value;
            }
        }


        public static string Password
        {
            get
            {
                return password;
            }
            set
            {
                if (UserName.Length <= 1)
                {
                    throw new ArgumentException("还没有指定好用户名。请先指定用户名");
                }


                password = value;
            }
        }


        public static void RemoteConfig(string hostName, string userName, string password)
        {
            HostName = hostName;
            UserName = userName;
            Password = password;
        }


        private static string hostName = "localhost";
        private static string userName;
        private static string password;
        #endregion


        #region 根据路径构造Entry的方法
        /// <summary> 
        /// 根据是否有用户名来判断是否是远程服务器。 
        /// 然后再构造出不同的DirectoryEntry出来 
        /// </summary> 
        /// <param name="entPath">DirectoryEntry的路径</param> 
        /// <returns>返回的是DirectoryEntry实例</returns> 
        public static DirectoryEntry GetDirectoryEntry(string entPath)
        {
            DirectoryEntry ent;


            if (UserName == null)
            {
                ent = new DirectoryEntry(entPath);
            }
            else
            {
                // ent = new DirectoryEntry(entPath, HostName+"\\"+UserName, Password, AuthenticationTypes.Secure); 
                ent = new DirectoryEntry(entPath, UserName, Password, AuthenticationTypes.Secure);
            }


            return ent;
        }
        #endregion


        #region 添加，删除网站的方法
        /// <summary> 
        /// 创建一个新的网站。根据传过来的信息进行配置 
        /// </summary> 
        /// <param name="siteInfo">存储的是新网站的信息</param> 
        public static void CreateNewWebSite(NewWebSiteInfo siteInfo)
        {
            if (!EnsureNewSiteEnavaible(siteInfo.BindString))
            {
                DeleteWebSiteByName(siteInfo.CommentOfWebSite);
                //throw new Exception("已经有了这样的网站了。" + Environment.NewLine + siteInfo.BindString);
            }


            string entPath = String.Format("IIS://{0}/w3svc", HostName);
            DirectoryEntry rootEntry = GetDirectoryEntry(entPath);


            string newSiteNum = GetNewWebSiteID();
            DirectoryEntry newSiteEntry = rootEntry.Children.Add(newSiteNum, "IIsWebServer");
            newSiteEntry.CommitChanges();


            newSiteEntry.Properties["ServerBindings"].Value = siteInfo.BindString;
            newSiteEntry.Properties["ServerComment"].Value = siteInfo.CommentOfWebSite;
            newSiteEntry.CommitChanges();


            DirectoryEntry vdEntry = newSiteEntry.Children.Add("root", "IIsWebVirtualDir");
            vdEntry.CommitChanges();

            //string ChangWebPath = siteInfo.WebPath.Trim().Remove(siteInfo.WebPath.Trim().LastIndexOf('\\'), 1);
            vdEntry.Properties["Path"].Value = siteInfo.WebPath.Trim();

            /*
            

            vdEntry.Properties["AccessRead"][0] = true; //设置读取权限
            vdEntry.Properties["AccessWrite"][0] = true;
            vdEntry.Properties["AccessScript"][0] = true;//执行权限
            vdEntry.Properties["AccessExecute"][0] = false;
            vdEntry.Properties["DefaultDoc"][0] = "Login.aspx";//设置默认文档
            vdEntry.Properties["AppFriendlyName"][0] = "LabManager"; //应用程序名称 
            vdEntry.Properties["AuthFlags"][0] = 1;//0表示不允许匿名访问,1表示就可以3为基本身份验证，7为windows继承身份验证
            vdEntry.Properties["EnableDefaultDoc"][0] = false;  //能否编辑默认起始文档
            vdEntry.Properties["EnableDirBrowsing"][0] = false; //可浏览目录
            vdEntry.Properties["AccessSSL"][0] = false;     //SSL设置
            vdEntry.Properties["AnonymousUserName"][0] = AnonymousUserName;  //匿名用户
            vdEntry.Properties["AnonymousUserPass"][0] = AnonymousUserPass;  //匿名密码
            */
            vdEntry.CommitChanges();

            //操作增加MIME
            /*
            IISOle.MimeMapClass NewMime = new IISOle.MimeMapClass();
            NewMime.Extension = ".xaml"; NewMime.MimeType = "application/xaml+xml";
            IISOle.MimeMapClass TwoMime = new IISOle.MimeMapClass();
            TwoMime.Extension = ".xap"; TwoMime.MimeType = "application/x-silverlight-app";
            rootEntry.Properties["MimeMap"].Add(NewMime);
            rootEntry.Properties["MimeMap"].Add(TwoMime);
            rootEntry.CommitChanges();
             * */

            #region 针对IIS7
            string entInfoPath = String.Format("IIS://{0}/w3svc/INFO", HostName);
            DirectoryEntry getInfoEntity = GetDirectoryEntry(entInfoPath);
            int Version = int.Parse(getInfoEntity.Properties["MajorIISVersionNumber"].Value.ToString());

            CreateAppPool(siteInfo.AppPoolName, siteInfo.NetVersion, Version > 6);

            if (!string.IsNullOrEmpty(siteInfo.AppPoolName))
            {
                vdEntry.Properties["AppPoolId"].Value = siteInfo.AppPoolName;
                vdEntry.CommitChanges();
            }
            #endregion

            if (!string.IsNullOrEmpty(siteInfo.NetPath))
            {
                //启动aspnet_regiis.exe程序 
                //string fileName = Environment.GetEnvironmentVariable("windir") + @"\Microsoft.NET\Framework\v4.0.30319\aspnet_regiis.exe";            
                ProcessStartInfo startInfo = new ProcessStartInfo(siteInfo.NetPath);

                //处理目录路径 
                string path = vdEntry.Path.ToUpper();
                int index = path.IndexOf("W3SVC");
                path = path.Remove(0, index);

                //启动ASPnet_iis.exe程序,刷新脚本映射 
                startInfo.Arguments = "-s " + path;
                startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                startInfo.UseShellExecute = false;
                startInfo.CreateNoWindow = true;
                startInfo.RedirectStandardOutput = true;
                startInfo.RedirectStandardError = true;

                Process process = new Process();
                process.StartInfo = startInfo;
                process.Start();

                process.WaitForExit();

                string errors = process.StandardError.ReadToEnd();
                if (errors != string.Empty)
                {
                    throw new Exception(errors);
                }
            }

            if (!string.IsNullOrEmpty(siteInfo.DescOfWebSite) && string.IsNullOrEmpty(siteInfo.HostIP))
            {
                AddToFile("127.0.0.1   " + siteInfo.DescOfWebSite, @"C:\WINDOWS\system32\drivers\etc\hosts");
            }
        }


        /// <summary> 
        /// 删除一个网站。根据网站名称删除。 
        /// </summary> 
        /// <param name="siteName">网站名称</param> 
        public static void DeleteWebSiteByName(string siteName)
        {
            string siteNum = GetWebSiteNum(siteName);
            string siteEntPath = String.Format("IIS://{0}/w3svc/{1}", HostName, siteNum);
            DirectoryEntry siteEntry = GetDirectoryEntry(siteEntPath);


            string rootPath = String.Format("IIS://{0}/w3svc", HostName);
            DirectoryEntry rootEntry = GetDirectoryEntry(rootPath);


            rootEntry.Children.Remove(siteEntry);
            rootEntry.CommitChanges();
        }
        #endregion


        #region Start和Stop网站的方法
        public static void StartWebSite(string siteName)
        {
            string siteNum = GetWebSiteNum(siteName);
            string siteEntPath = String.Format("IIS://{0}/w3svc/{1}", HostName, siteNum);
            DirectoryEntry siteEntry = GetDirectoryEntry(siteEntPath);


            siteEntry.Invoke("Start", new object[] { });
        }


        public static void StopWebSite(string siteName)
        {
            string siteNum = GetWebSiteNum(siteName);
            string siteEntPath = String.Format("IIS://{0}/w3svc/{1}", HostName, siteNum);
            DirectoryEntry siteEntry = GetDirectoryEntry(siteEntPath);


            siteEntry.Invoke("Stop", new object[] { });
        }
        #endregion

        #region 确认网站是否相同
        /// <summary> 
        /// 确定一个新的网站与现有的网站没有相同的。 
        /// 这样防止将非法的数据存放到IIS里面去 
        /// </summary> 
        /// <param name="bindStr">网站邦定信息</param> 
        /// <returns>真为可以创建，假为不可以创建</returns> 
        public static bool EnsureNewSiteEnavaible(string bindStr)
        {
            string entPath = String.Format("IIS://{0}/w3svc", HostName);
            DirectoryEntry ent = GetDirectoryEntry(entPath);


            foreach (DirectoryEntry child in ent.Children)
            {
                if (child.SchemaClassName == "IIsWebServer")
                {
                    if (child.Properties["ServerBindings"].Value != null)
                    {
                        if (child.Properties["ServerBindings"].Value.ToString() == bindStr)
                        {
                            return false;
                        }
                    }
                }
            }


            return true;
        }
        #endregion

        #region 获取一个网站编号的方法
        /// <summary> 
        /// 获取一个网站的编号。根据网站的ServerBindings或者ServerComment来确定网站编号 
        /// </summary> 
        /// <param name="siteName"></param> 
        /// <returns>返回网站的编号</returns> 
        /// <exception cref="NotFoundWebSiteException">表示没有找到网站</exception> 
        public static string GetWebSiteNum(string siteName)
        {
            Regex regex = new Regex(siteName);
            string tmpStr;


            string entPath = String.Format("IIS://{0}/w3svc", HostName);
            DirectoryEntry ent = GetDirectoryEntry(entPath);


            foreach (DirectoryEntry child in ent.Children)
            {
                if (child.SchemaClassName == "IIsWebServer")
                {
                    if (child.Properties["ServerBindings"].Value != null)
                    {
                        tmpStr = child.Properties["ServerBindings"].Value.ToString();
                        if (regex.Match(tmpStr).Success)
                        {
                            return child.Name;
                        }
                    }


                    if (child.Properties["ServerComment"].Value != null)
                    {
                        tmpStr = child.Properties["ServerComment"].Value.ToString();
                        if (regex.Match(tmpStr).Success)
                        {
                            return child.Name;
                        }
                    }
                }
            }


            throw new Exception("没有找到我们想要的站点" + siteName);
        }
        #endregion


        #region 获取新网站id的方法
        /// <summary> 
        /// 获取网站系统里面可以使用的最小的ID。 
        /// 这是因为每个网站都需要有一个唯一的编号，而且这个编号越小越好。 
        /// 这里面的算法经过了测试是没有问题的。 
        /// </summary> 
        /// <returns>最小的id</returns> 
        public static string GetNewWebSiteID()
        {
            ArrayList list = new ArrayList();
            string tmpStr;


            string entPath = String.Format("IIS://{0}/w3svc", HostName);
            DirectoryEntry ent = GetDirectoryEntry(entPath);


            foreach (DirectoryEntry child in ent.Children)
            {
                if (child.SchemaClassName == "IIsWebServer")
                {
                    tmpStr = child.Name.ToString();
                    list.Add(Convert.ToInt32(tmpStr));
                }
            }


            list.Sort();


            int i = 1;
            foreach (int j in list)
            {
                if (i == j)
                {
                    i++;
                }
            }


            return i.ToString();
        }
        #endregion

        /// <summary>
        /// 判断程序池是否存在
        /// </summary>
        /// <param name="AppPoolName">程序池名称</param>
        /// <returns>true存在 false不存在</returns>
        public static bool IsAppPoolName(string AppPoolName)
        {
            bool result = false;

            string entPath = String.Format("IIS://{0}/w3svc/AppPools", HostName);
            DirectoryEntry appPools = GetDirectoryEntry(entPath);

            foreach (DirectoryEntry getdir in appPools.Children)
            {
                if (getdir.Name.Equals(AppPoolName))
                {
                    result = true;
                }
            }
            return result;
        }
        /// <summary>
        /// 删除指定程序池
        /// </summary>
        /// <param name="AppPoolName">程序池名称</param>
        /// <returns>true删除成功 false删除失败</returns>
        public static bool DeleteAppPool(string AppPoolName)
        {
            bool result = false;

            string entPath = String.Format("IIS://{0}/w3svc/AppPools", HostName);
            DirectoryEntry appPools = GetDirectoryEntry(entPath);
            foreach (DirectoryEntry getdir in appPools.Children)
            {
                if (getdir.Name.Equals(AppPoolName))
                {
                    try
                    {
                        getdir.DeleteTree();
                        result = true;
                    }
                    catch
                    {
                        result = false;
                    }
                }
            }
            return result;
        }

        public static void CreateAppPool(string AppPoolName, string Version, bool isIIS7)
        {
            if (!string.IsNullOrEmpty(AppPoolName))
            {
                if (IsAppPoolName(AppPoolName))
                    DeleteAppPool(AppPoolName);
                DirectoryEntry newpool;
                string entPath = String.Format("IIS://{0}/w3svc/AppPools", HostName);
                DirectoryEntry appPools = GetDirectoryEntry(entPath);
                newpool = appPools.Children.Add(AppPoolName, "IIsApplicationPool");
                newpool.CommitChanges();
            }

            if (isIIS7 && !string.IsNullOrEmpty(Version))
            {
                ServerManager sm = new ServerManager();
                sm.ApplicationPools[AppPoolName].ManagedRuntimeVersion = Version;//  "v4.0"
                sm.ApplicationPools[AppPoolName].ManagedPipelineMode = ManagedPipelineMode.Classic; //托管模式Integrated为集成 Classic为经典
                sm.CommitChanges();
            }
        }

        private static bool AddToFile(string content, string filePath)
        {
            FileStream fs = null;
            try
            {
                byte[] bData = System.Text.Encoding.UTF8.GetBytes(content.ToString());

                fs = new FileStream(filePath, FileMode.Append);//FileMode.CreateNew
                fs.Write(bData, 0, bData.Length);
            }
            catch
            {
                return false;
            }
            finally
            {
                if (fs != null)
                {
                    fs.Close();
                    fs.Dispose();
                }
            }
            return true;
        }
    }


    #region 新网站信息结构体
    public struct NewWebSiteInfo
    {
        private string hostIP; // The Hosts IP Address
        private string portNum; // The New Web Sites Port.generally is "80"
        private string descOfWebSite; // 网站表示。一般为网站的网站名。例如"www.dns.com.cn"
        private string commentOfWebSite;// 网站注释。一般也为网站的网站名。
        private string webPath; // 网站的主目录。例如"e:\tmp"
        private string appPoolName;
        private string netVersion;
        private string netPath;

        //NewWebSiteInfo website = new NewWebSiteInfo("", "84", "", 
        //                      "webName", @"D:\TestASP\WebApplication2010", "webName", "v4.0");
        public NewWebSiteInfo(string hostIP, string portNum, string descOfWebSite,
            string commentOfWebSite, string webPath,
            string appPoolName, string netVersion, string netPath)
        {
            this.hostIP = hostIP;
            this.portNum = portNum;
            this.descOfWebSite = descOfWebSite;
            this.commentOfWebSite = commentOfWebSite;
            this.webPath = webPath;
            this.appPoolName = appPoolName;
            this.netVersion = netVersion;
            this.netPath = netPath;
        }

        public string BindString
        {
            get
            {
                return String.Format("{0}:{1}:{2}", hostIP, portNum, descOfWebSite);
            }
        }

        public string HostIP
        {
            get { return hostIP; }
        }
        public string DescOfWebSite
        {
            get
            {
                return descOfWebSite;
            }
        }
        public string CommentOfWebSite
        {
            get
            {
                return commentOfWebSite;
            }
        }

        public string WebPath
        {
            get
            {
                return webPath;
            }
        }

        public string AppPoolName
        {
            get
            {
                return appPoolName;
            }
        }

        public string NetVersion
        {
            get
            {
                return netVersion;
            }
        }

        public string NetPath
        {
            get
            {
                return netPath;
            }
        }

    }
    #endregion
}

