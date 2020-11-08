using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GK2823.BizLib.Admin.Services
{
    public class H3CService
    {
        public static string GetContentFromTxt(string fileName)
        {
            try
            {
                string line = string.Empty;
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                //2.获取和设置当前目录(该进程从中启动的目录)的完全限定目录 
                string path2 = System.Environment.CurrentDirectory;

                //3.获取应用程序的当前工作目录 
                string path3 = System.IO.Directory.GetCurrentDirectory();

                //4.获取程序的基目录 
                string path4 = System.AppDomain.CurrentDomain.BaseDirectory;
                string fileUrl = Path.Combine("E:\\Main\\gk2823_files\\txt", fileName);
                
                line = File.ReadAllText(fileUrl, Encoding.GetEncoding("UTF-8"));
                return line;
            }
            catch (Exception ex)
            {
                return ex.Message + ex.StackTrace;
            }
        }

        public object H3COasisTableInfo()
        {
            return H3CService.GetContentFromTxt("全景图20201105.txt");
        }


    }
}
