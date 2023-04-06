using GenshinWish.Common;
using GenshinWish.Util;
using Quartz;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GenshinWish.Timer
{
    public class ClearJob : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            return Task.Run(() =>
            {
                try
                {
                    LogHelper.Info($"开始清理历史图片...");
                    string clearPath = ApiConfig.ImgSavePath;
                    if (Directory.Exists(clearPath) == false)
                    {
                        LogHelper.Info($"清理目录不存在...");
                        return;
                    }
                    string[] dirPathArr = Directory.GetDirectories(clearPath);
                    if (dirPathArr == null || dirPathArr.Length == 0)
                    {
                        LogHelper.Info($"清理目录为空...");
                        return;
                    }
                    string todayDirName = $"{DateTime.Now.ToString("yyyyMMdd")}";
                    foreach (var item in dirPathArr)
                    {
                        var dirInfo = new DirectoryInfo(item);
                        if (dirInfo.Name == todayDirName) continue;
                        DelDir(dirInfo.FullName);
                    }
                    LogHelper.Info($"清理历史图片完毕...");
                }
                catch (Exception ex)
                {
                    LogHelper.Error(ex);
                }
            });
        }

        private void DelDir(string dirPath)
        {
            try
            {
                Directory.Delete(dirPath, true);
            }
            catch (Exception ex)
            {
                LogHelper.Info($"定时清理历史图片目录{dirPath}失败");
                LogHelper.Error(ex);
            }
        }


    }
}
