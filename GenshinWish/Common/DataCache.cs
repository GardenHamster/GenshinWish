using GenshinWish.Models.BO;
using GenshinWish.Models.VO;
using System;
using System.Collections.Generic;

namespace GenshinWish.Common
{
    public static class DataCache
    {
        /// <summary>
        /// 5星角色常驻列表
        /// </summary>
        public static List<GoodsItemBO> RoleStar5PermList = new List<GoodsItemBO>();

        /// <summary>
        /// 4星角色常驻列表
        /// </summary>
        public static List<GoodsItemBO> RoleStar4PermList = new List<GoodsItemBO>();

        /// <summary>
        /// 5星武器常驻列表
        /// </summary>
        public static List<GoodsItemBO> ArmStar5PermList = new List<GoodsItemBO>();

        /// <summary>
        /// 4星武器常驻列表
        /// </summary>
        public static List<GoodsItemBO> ArmStar4PermList = new List<GoodsItemBO>();

        /// <summary>
        /// 3星武器常驻列表
        /// </summary>
        public static List<GoodsItemBO> ArmStar3PermList = new List<GoodsItemBO>();

        /// <summary>
        /// 4星常驻列表
        /// </summary>
        public static List<GoodsItemBO> Star4PermList = new List<GoodsItemBO>();

        /// <summary>
        /// 5星常驻列表
        /// </summary>
        public static List<GoodsItemBO> Star5PermList = new List<GoodsItemBO>();

        /// <summary>
        /// 默认常驻池
        /// </summary>
        public static UpItemBO DefaultPermItem = new UpItemBO();

        /// <summary>
        /// 默认武器池
        /// </summary>
        public static Dictionary<int, UpItemBO> DefaultArmItem = new Dictionary<int, UpItemBO>();

        /// <summary>
        /// 默认角色池
        /// </summary>
        public static Dictionary<int, UpItemBO> DefaultRoleItem = new Dictionary<int, UpItemBO>();

        /// <summary>
        /// 全武器池
        /// </summary>
        public static UpItemBO FullArmItem = new UpItemBO();

        /// <summary>
        /// 全角色池
        /// </summary>
        public static UpItemBO FullRoleItem = new UpItemBO();

        /// <summary>
        /// 排行缓存
        /// </summary>
        private static Dictionary<int, LuckRankingVO> RankingCache = new Dictionary<int, LuckRankingVO>();

        /// <summary>
        /// 从缓存中获取欧气排行
        /// </summary>
        /// <param name="authId"></param>
        /// <returns></returns>
        public static LuckRankingVO GetLuckRankingCache(int authId)
        {
            if (RankingCache.ContainsKey(authId) == false) return null;
            if (RankingCache[authId] == null) return null;
            LuckRankingVO luckRankingVO = RankingCache[authId];
            if (luckRankingVO.CacheDate.AddMinutes(ApiConfig.RankingCacheMinutes) < DateTime.Now) return null;
            return luckRankingVO;
        }

        /// <summary>
        /// 将欧气排行放入缓存
        /// </summary>
        /// <param name="authId"></param>
        /// <param name="luckRankingVO"></param>
        public static void SetLuckRankingCache(int authId, LuckRankingVO luckRankingVO)
        {
            luckRankingVO.CacheDate = DateTime.Now;
            RankingCache[authId] = luckRankingVO;
        }

    }
}
