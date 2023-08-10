using GenshinWish.Models.BO;
using GenshinWish.Type;
using System;
using System.Collections.Generic;
using System.IO;

namespace GenshinWish.Common
{
    /// <summary>
    /// 
    /// </summary>
    public static class FilePath
    {

        /// <summary>
        /// 获取模拟抽卡图片保存的绝对路径
        /// </summary>
        /// <returns></returns>
        public static string getImgSavePath()
        {
            DateTime dateTime = DateTime.Now;
            string path = Path.Combine(ApiConfig.ImgSavePath, $"{dateTime.ToString("yyyyMMdd")}", $"{dateTime.ToString("HH")}");
            if (Directory.Exists(path) == false) Directory.CreateDirectory(path);
            return path;
        }

        /// <summary>
        /// 背景图路径
        /// </summary>
        /// <returns></returns>
        public static string getFontPath()
        {
            return Path.Combine(ApiConfig.MaterialSavePath, "Fonts", "HYWenHei-85W.ttf");
        }

        /// <summary>
        /// 背景图路径
        /// </summary>
        /// <returns></returns>
        public static string getBackgroundPath()
        {
            return Path.Combine(ApiConfig.MaterialSavePath, "背景", "背景.png");
        }

        /// <summary>
        /// 框路径
        /// </summary>
        /// <returns></returns>
        public static string getFrameImgPath()
        {
            return Path.Combine(ApiConfig.MaterialSavePath, "框", "框.png");
        }

        /// <summary>
        /// 星星路径
        /// </summary>
        /// <returns></returns>
        public static string getYSProspectImgPath()
        {
            return Path.Combine(ApiConfig.MaterialSavePath, "框", "星星.png");
        }

        /// <summary>
        /// 角色小图路径
        /// </summary>
        /// <param name="goodsItem"></param>
        /// <param name="isUseSkin"></param>
        /// <returns></returns>
        public static string getYSSmallRoleImgPath(GoodsItemBO goodsItem, bool isUseSkin)
        {
            string generalPath = Path.Combine(ApiConfig.MaterialSavePath, "角色小图", $"{goodsItem.GoodsName}.png");
            if (isUseSkin == false) return generalPath;
            string skinDirPath = Path.Combine(ApiConfig.MaterialSavePath, "服装小图", $"{goodsItem.GoodsName}");
            if (Directory.Exists(skinDirPath) == false) return generalPath;
            DirectoryInfo directoryInfo = new DirectoryInfo(skinDirPath);
            FileInfo[] files = directoryInfo.GetFiles();
            if (files == null || files.Length == 0) return generalPath;
            int randomIndex = new Random().Next(files.Length);
            return files[randomIndex].FullName;
        }

        /// <summary>
        /// 武器大图路径
        /// </summary>
        /// <param name="goodsItem"></param>
        /// <returns></returns>
        public static string getYSEquipImgPath(GoodsItemBO goodsItem)
        {
            return Path.Combine(ApiConfig.MaterialSavePath, "武器", $"{goodsItem.GoodsName}.png");
        }

        /// <summary>
        /// 光效图片路径
        /// </summary>
        /// <param name="goodsItem"></param>
        /// <returns></returns>
        public static string getYSLightPath(GoodsItemBO goodsItem)
        {
            if (goodsItem.RareType == RareType.五星)
            {
                string dirPath = Path.Combine(ApiConfig.MaterialSavePath, "框", "金色框");
                return getRandomInDir(dirPath).FullName;
            }
            if (goodsItem.RareType == RareType.四星)
            {
                string dirPath = Path.Combine(ApiConfig.MaterialSavePath, "框", "紫色框");
                return getRandomInDir(dirPath).FullName;
            }
            if (goodsItem.RareType == RareType.三星)
            {
                string dirPath = Path.Combine(ApiConfig.MaterialSavePath, "框", "蓝色框");
                return getRandomInDir(dirPath).FullName;
            }
            throw new Exception($"找不到与{Enum.GetName(typeof(GoodsItemBO), goodsItem.RareType)}对应的光效图");
        }

        /// <summary>
        /// 星星路径
        /// </summary>
        /// <returns></returns>
        public static string getYSStarPath()
        {
            return Path.Combine(ApiConfig.MaterialSavePath, "图标", "星星.png");
        }

        /// <summary>
        /// 花纹路径
        /// </summary>
        /// <returns></returns>
        public static string getYSShadingPath()
        {
            return Path.Combine(ApiConfig.MaterialSavePath, "框", "花纹.png");
        }

        /// <summary>
        /// 透明元素图标路径
        /// </summary>
        /// <param name="goodsItem"></param>
        /// <returns></returns>
        public static string getYSBigElementIconPath(GoodsItemBO goodsItem)
        {
            return Path.Combine(ApiConfig.MaterialSavePath, "元素图标大", $"{Enum.GetName(typeof(GoodsSubType), goodsItem.GoodsSubType)}.png");
        }

        /// <summary>
        /// 彩色元素图标路径
        /// </summary>
        /// <param name="goodsItem"></param>
        /// <returns></returns>
        public static string getYSSmallElementIconPath(GoodsItemBO goodsItem)
        {
            return Path.Combine(ApiConfig.MaterialSavePath, "元素图标小", $"{Enum.GetName(typeof(GoodsSubType), goodsItem.GoodsSubType)}.png");
        }

        /// <summary>
        /// 白色武器图标路径
        /// </summary>
        /// <param name="goodsItem"></param>
        /// <returns></returns>
        public static string getYSWhiteEquipIconPath(GoodsItemBO goodsItem)
        {
            return Path.Combine(ApiConfig.MaterialSavePath, "武器图标白", $"{Enum.GetName(typeof(GoodsSubType), goodsItem.GoodsSubType)}.png");
        }

        /// <summary>
        /// 灰色武器图标路径
        /// </summary>
        /// <param name="goodsItem"></param>
        /// <returns></returns>
        public static string getYSBlackEquipIconPath(GoodsItemBO goodsItem)
        {
            return Path.Combine(ApiConfig.MaterialSavePath, "武器图标黑", $"{Enum.GetName(typeof(GoodsSubType), goodsItem.GoodsSubType)}.png");
        }

        /// <summary>
        /// 关闭图标路径
        /// </summary>
        /// <returns></returns>
        public static string getYSCloseIconPath()
        {
            return Path.Combine(ApiConfig.MaterialSavePath, "图标", "关闭.png");
        }

        /// <summary>
        /// 关闭图标路径
        /// </summary>
        /// <returns></returns>
        public static string getYSNewIconPath()
        {
            return Path.Combine(ApiConfig.MaterialSavePath, "图标", "new.png");
        }

        /// <summary>
        /// 星尘图标路径
        /// </summary>
        /// <returns></returns>
        public static string getYSStarDustIconPath(GoodsItemBO goodsItem)
        {
            if (goodsItem.RareType == RareType.四星)
            {
                return Path.Combine(ApiConfig.MaterialSavePath, "图标", "星尘紫.png");
            }
            if (goodsItem.RareType == RareType.五星)
            {
                return Path.Combine(ApiConfig.MaterialSavePath, "图标", "星尘金.png");
            }
            throw new Exception($"找不到与{Enum.GetName(typeof(GoodsItemBO), goodsItem.RareType)}对应的星尘素材");
        }

        /// <summary>
        /// 星辉图标路径
        /// </summary>
        /// <returns></returns>
        public static string getYSStarLightIconPath(int count)
        {
            if (count == 2) return Path.Combine(ApiConfig.MaterialSavePath, "图标", "星辉2.png");
            if (count == 5) return Path.Combine(ApiConfig.MaterialSavePath, "图标", "星辉5.png");
            if (count == 10) return Path.Combine(ApiConfig.MaterialSavePath, "图标", "星辉10.png");
            if (count == 25) return Path.Combine(ApiConfig.MaterialSavePath, "图标", "星辉25.png");
            throw new Exception($"找不到数量为{count}的星辉素材");
        }

        /// <summary>
        /// 角色大图路径
        /// </summary>
        /// <param name="goodsItem"></param>
        /// <param name="withSkin"></param>
        /// <returns></returns>
        public static string getYSBigRoleImgPath(GoodsItemBO goodsItem, bool withSkin)
        {
            string generalPath = Path.Combine(ApiConfig.MaterialSavePath, "角色大图", $"{goodsItem.GoodsName}.png");
            if (withSkin == false) return generalPath;
            string skinDirPath = Path.Combine(ApiConfig.MaterialSavePath, "服装大图", $"{goodsItem.GoodsName}");
            if (Directory.Exists(skinDirPath) == false) return generalPath;
            DirectoryInfo directoryInfo = new DirectoryInfo(skinDirPath);
            FileInfo[] files = directoryInfo.GetFiles();
            if (files == null || files.Length == 0) return generalPath;
            int randomIndex = new Random().Next(files.Length);
            return files[randomIndex].FullName;
        }

        /// <summary>
        /// 单抽中的武器背景图片路径
        /// </summary>
        /// <param name="goodsItem"></param>
        /// <returns></returns>
        public static string getYSEquipBgPath(GoodsItemBO goodsItem)
        {
            return Path.Combine(ApiConfig.MaterialSavePath, "武器背景", $"{Enum.GetName(typeof(GoodsSubType), goodsItem.GoodsSubType)}.png");
        }

        /// <summary>
        /// 代币图标路径
        /// </summary>
        /// <param name="goodsItem"></param>
        /// <returns></returns>
        public static string getYSTokenPath(GoodsItemBO goodsItem)
        {
            if (goodsItem.RareType == RareType.三星) return Path.Combine(ApiConfig.MaterialSavePath, "框", "无主的星尘15.png");
            if (goodsItem.RareType == RareType.四星) return Path.Combine(ApiConfig.MaterialSavePath, "框", "无主的星辉02.png");
            if (goodsItem.RareType == RareType.五星) return Path.Combine(ApiConfig.MaterialSavePath, "框", "无主的星辉10.png");
            throw new Exception($"找不到与{Enum.GetName(typeof(RareType), goodsItem.RareType)}对应的代币");
        }

        /// <summary>
        /// 泡泡路径
        /// </summary>
        /// <returns></returns>
        public static List<string> getBubblesBigPathList()
        {
            return new List<string>()
            {
                Path.Combine(ApiConfig.MaterialSavePath, "泡泡", "蓝色10.png"),
                Path.Combine(ApiConfig.MaterialSavePath, "泡泡", "紫色10.png"),
                Path.Combine(ApiConfig.MaterialSavePath, "泡泡", "蓝色05.png"),
                Path.Combine(ApiConfig.MaterialSavePath, "泡泡", "紫色05.png")
            };
        }

        /// <summary>
        /// 泡泡路径
        /// </summary>
        /// <returns></returns>
        public static List<string> getBubblesSmallPathList()
        {
            return new List<string>()
            {
                Path.Combine(ApiConfig.MaterialSavePath, "泡泡", "蓝色50.png"),
                Path.Combine(ApiConfig.MaterialSavePath, "泡泡", "紫色50.png"),
                Path.Combine(ApiConfig.MaterialSavePath, "泡泡", "蓝色25.png"),
                Path.Combine(ApiConfig.MaterialSavePath, "泡泡", "紫色25.png")
            };
        }

        /// <summary>
        /// 从一个文件夹中随机获取一个文件
        /// </summary>
        /// <param name="dirPath"></param>
        /// <returns></returns>
        private static FileInfo getRandomInDir(string dirPath)
        {
            DirectoryInfo sourceDirectory = new DirectoryInfo(dirPath);
            FileInfo[] fileInfos = sourceDirectory.GetFiles();
            int randomFileIndex = new Random().Next(fileInfos.Length);
            return fileInfos[randomFileIndex];
        }


    }
}
