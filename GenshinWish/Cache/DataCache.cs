using GenshinWish.Common;
using GenshinWish.Helper;
using GenshinWish.Models.VO;
using System;
using System.Collections.Generic;

namespace GenshinWish.Cache
{
    public static class DataCache
    {
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
            if (luckRankingVO.CacheTime + ApiConfig.RankingCacheMinutes * 60 < DateTimeHelper.GetTimeStamp()) return null;
            return luckRankingVO;
        }

        /// <summary>
        /// 将欧气排行放入缓存
        /// </summary>
        /// <param name="authId"></param>
        /// <param name="luckRankingVO"></param>
        public static void SetLuckRankingCache(int authId, LuckRankingVO luckRankingVO)
        {
            luckRankingVO.CacheTime = DateTimeHelper.GetTimeStamp();
            RankingCache[authId] = luckRankingVO;
        }

    }
}
