using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//C:\Windows\System32\inetsrv or C:\Windows\SysWOW64\inetsrv\Microsoft.Web.Administration.dll
using Microsoft.Web.Administration;

namespace ConsoleTestAppPool
{
    class Program
    {
        static void Main(string[] args)
        {
            //需要使用管理员身份运行,否则new ServerManager()提示无权限
            using (ServerManager serverManager = new ServerManager())
            {
                //Site site = serverManager.Sites["sass"]; 获取现有site
                Site newSite = CreateSite(serverManager, "racing_site", "C:/inetpub/wwwroot/racing", 8899);
                CreateAppPool(serverManager, "racing");
                CreateAppPool(serverManager, "racing_test");
                CreateAppPool(serverManager, "racing_test_tow");

                CreateApplication(newSite, "d:/racing", "/racing", "racing");
                CreateApplication(newSite, "d:/racing/test", "/racing/test", "racing_test");
                CreateApplication(newSite, "d:/racing/test_tow", "/racing/test_tow", "racing_test_tow");

                // 一次性提交
                serverManager.CommitChanges();

                Console.WriteLine("ok");
                Console.ReadLine();

            }
        }

        /// <summary>
        /// 创建应用程序池
        /// </summary>
        /// <param name="serverManager"></param>
        /// <param name="poolName"></param>
        static void CreateAppPool(ServerManager serverManager, string poolName)
        {
            ApplicationPool newPool = serverManager.ApplicationPools.Add(poolName);
            newPool.ManagedRuntimeVersion = "v4.0";// or v2.0
            newPool.ManagedPipelineMode = ManagedPipelineMode.Classic;
            // 默认为 ProcessModelIdentityType.ApplicationPoolIdentity
            newPool.ProcessModel.IdentityType = ProcessModelIdentityType.SpecificUser;
            newPool.ProcessModel.UserName = "domain\\user_name";
            newPool.ProcessModel.Password = "user_password";
        }
        /// <summary>
        /// 创建站点
        /// </summary>
        /// <param name="serverManager"></param>
        /// <param name="siteName"></param>
        /// <param name="physicalPath"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        static Site CreateSite(ServerManager serverManager, string siteName, string physicalPath, int port)
        {
            Site mySite = serverManager.Sites.Add(siteName, physicalPath, port);
            return mySite;
        }
        /// <summary>
        /// 创建应用网站
        /// </summary>
        /// <param name="site"></param>
        /// <param name="physicalPath"></param>
        /// <param name="path"></param>
        /// <param name="poolName"></param>
        static void CreateApplication(Site site, string physicalPath, string path = "/", string poolName = null)
        {
            var app = site.Applications.Add(path, physicalPath);
            if (!string.IsNullOrEmpty(poolName))
            {
                app.ApplicationPoolName = poolName;
            }
        }
    }
}
