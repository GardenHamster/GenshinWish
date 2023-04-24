using GenshinWish.Common;
using GenshinWish.Models.DTO;
using Microsoft.AspNetCore.Builder;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;

namespace GenshinWish.Helper
{
    public static class ConfigHelper
    {
        /// <summary>
        /// 加载appsettings.json
        /// </summary>
        /// <param name="builder"></param>
        public static void LoadAppSettings(WebApplicationBuilder builder)
        {
            ApiConfig.ConnectionString = builder.Configuration.GetSection("ConnectionString").Value;
            ApiConfig.ImgSavePath = builder.Configuration.GetSection("ImgSavePath").Value;
            ApiConfig.MaterialSavePath = builder.Configuration.GetSection("MaterialSavePath").Value;
            ApiConfig.ImgHttpUrl = builder.Configuration.GetSection("ImgHttpUrl").Value;
            ApiConfig.PublicAuthCode = builder.Configuration.GetSection("PublicAuthCode").Value;
        }

        /// <summary>
        /// 加载InitData.json
        /// </summary>
        /// <returns></returns>
        public static InitDataDto LoadInitData()
        {
            string jsonPath = Path.Combine(ApiConfig.MaterialSavePath, "InitData.json");
            if (File.Exists(jsonPath) == false) return null;
            var jsonStr = File.ReadAllText(jsonPath, Encoding.UTF8);
            return JsonConvert.DeserializeObject<InitDataDto>(jsonStr);
        }


    }
}
