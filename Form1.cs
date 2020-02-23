using Newtonsoft.Json;
using PwC.Tax.Tech.Atms.Globals.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Deployment
{
    public partial class Form1 : Form
    {
        private readonly string BIN = AppDomain.CurrentDomain.BaseDirectory;
        private readonly string BINRoot = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "wwwroot");
        private readonly string FileTemplatePath = ConfigurationManager.AppSettings["FileTemplate"];
        private readonly string LineSeparator = ConfigurationManager.AppSettings["LineSeparator"];
        private readonly bool IsLogDetail = ConfigurationManager.AppSettings["IsLogDetail"] == null ? false : Convert.ToBoolean(ConfigurationManager.AppSettings["IsLogDetail"].ToString());

        private readonly string ByPassFiles = ConfigurationManager.AppSettings["ByPassFiles"];

        private readonly List<string> ByPassFileList = new List<string>();

        private readonly List<FileData> FileConfigList = new List<FileData>();
        public Form1()
        {
            InitializeComponent();
            FileConfigList.Clear();
            FileConfigList = GetFileConfig();
            foreach (var data in FileConfigList)
            {
                lbLog.Items.Add("From:" + data.From + "----- To:" + data.To);
            }
            if (!string.IsNullOrEmpty(ByPassFiles))
            {
                var fileArr = ByPassFiles.Split(new string[] { ";", "," }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var f in fileArr)
                {
                    ByPassFileList.Add(f);
                }
            }
        }

        private void btnRun_Click(object sender, EventArgs e)
        {
            this.btnRun.Enabled = false;
            LogHelper.LogInfo("Begin to deployment");
            List<FileData> fileDatList = FileConfigList;
            Task[] tasks = new Task[fileDatList.Count];
            try
            {

                for (var i = 0; i < fileDatList.Count; i++)
                {
                    try
                    {
                        var data = fileDatList[i];
                        var msg = "Begin to copy: " + data.From + " To:" + data.To;
                        lbLog.Items.Add(msg);
                        tasks[i] = new Task(() =>
                        {
                            LogHelper.LogInfo(msg);
                            DirectoryCopy(data.From, data.To, true);
                        });

                    }
                    catch (Exception ex)
                    {
                        lbLog.Items.Add(ex.Message);
                        LogHelper.LogError(ex);
                    }

                }
                Parallel.ForEach<Task>(tasks, (t) => { t.Start(); });
               //  Wait for all the tasks to finish.
                Task.WaitAll(tasks);
                //foreach (var data in fileDatList)
                //{
                //    try
                //    {
                //        var msg = "Begin to copy: " + data.From + " To:" + data.To;
                //        lbLog.Items.Add(msg);
                //        LogHelper.LogInfo(msg);
                //        DirectoryCopy(data.From, data.To, true);
                //    }
                //    catch (Exception ex)
                //    {
                //        lbLog.Items.Add(ex.Message);
                //        LogHelper.LogError(ex);
                //    }
                //}
            }
            catch (Exception ex)
            {
                lbLog.Items.Add(ex);
                LogHelper.LogError(ex);
            }
            // Wait for all the tasks to finish.
            //Task.WaitAll(tasks);
            MessageBox.Show("Deployment Done","Information",MessageBoxButtons.OK);
            LogHelper.LogInfo("End to Deployment ");
            this.btnRun.Enabled = true;
         
        }
        private List<FileData> GetFileConfig()
        {
            var list = new List<FileData>();
            var data = new FileData();
            foreach (string line in File.ReadLines(FileTemplatePath, Encoding.UTF8))
            {
                if (line.StartsWith("#") || line.StartsWith("/") || line.StartsWith("@") || string.IsNullOrEmpty(line))
                {
                    continue;
                }
                var linArray = line.Split(new string[] { LineSeparator }, StringSplitOptions.RemoveEmptyEntries);
                var s1 = linArray[0].Split(new string[] { "==" }, StringSplitOptions.RemoveEmptyEntries);
                var s2 = linArray[1].Split(new string[] { "==" }, StringSplitOptions.RemoveEmptyEntries);
                var from = s1[1].Trim();
                var to = s2[1].Trim();
                list.Add(new FileData()
                {
                    From = Path.Combine(BINRoot, from),
                    To = to
                });
            }

            return list;
        } 

        private  void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            // Get the subdirectories for the specified directory.
            if (IsLogDetail)
            {
                var msg = "Begin to copy: " + sourceDirName + " To:" + destDirName;
                LogHelper.LogInfo(msg);
            }

            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            DirectoryInfo[] dirs = dir.GetDirectories();
            // If the destination directory doesn't exist, create it.
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                //by pass specific files
                if (ByPassFileList.Contains(file.Name))
                {
                    continue;
                }

                string temppath = Path.Combine(destDirName, file.Name);
                try
                {
                    if (File.Exists(temppath))
                    {
                        File.SetAttributes(temppath, FileAttributes.Normal);
                    }
                    file.CopyTo(temppath, true);
                }
                catch (Exception ex)
                {
                    lbLog.Items.Add("Copy File:" + temppath + "failed,Error:" + ex.Message);
                    LogHelper.LogError(ex);
                }
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }
        }

        public class FileData {
            public string From { get; set; }
            public string To { get; set; }
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            this.Close();
            Application.Exit();
        }
    }
}
