namespace GenshinWish.Common
{
    public static class ApiConfig
    {
        /// <summary>
        /// 公用授权码
        /// </summary>
        public static string PublicAuthCode = "theresa3rd";

        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        public static string ConnectionString = "";

        /// <summary>
        /// 祈愿结果图片保存目录
        /// </summary>
        public static string ImgSavePath = "";

        /// <summary>
        /// 祈愿素材保存路径
        /// </summary>
        public static string MaterialSavePath = "";

        /// <summary>
        /// 祈愿结果图片http路径
        /// </summary>
        public static string ImgHttpUrl = "";

        /// <summary>
        /// 排行统计缓存时间(分钟)
        /// </summary>
        public static int RankingCacheMinutes = 5;

    }
}
