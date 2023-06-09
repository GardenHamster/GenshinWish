﻿using GenshinWish.Cache;
using GenshinWish.Dao;
using GenshinWish.Helper;
using GenshinWish.Models.BO;
using GenshinWish.Models.DTO;
using GenshinWish.Models.PO;
using GenshinWish.Models.VO;
using GenshinWish.Type;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GenshinWish.Service
{
    public class ReceiveRecordService : BaseService
    {
        private WishRecordDao wishRecordDao;
        private MemberGoodsDao memberGoodsDao;
        private ReceiveRecordDao receiveRecordDao;

        public ReceiveRecordService(WishRecordDao wishRecordDao, MemberGoodsDao memberGoodsDao, ReceiveRecordDao receiveRecordDao)
        {
            this.wishRecordDao = wishRecordDao;
            this.memberGoodsDao = memberGoodsDao;
            this.receiveRecordDao = receiveRecordDao;
        }

        public WishDetailDto GetWishDetail(int memberId)
        {
            WishDetailDto wishDetail = new WishDetailDto();
            wishDetail.Star3Count = memberGoodsDao.CountGoods(memberId, RareType.三星);
            wishDetail.Star4Count = memberGoodsDao.CountGoods(memberId, RareType.四星);
            wishDetail.Star5Count = memberGoodsDao.CountGoods(memberId, RareType.五星);
            wishDetail.CharStar4Rate = receiveRecordDao.CountRate(memberId, PoolType.角色, RareType.四星);
            wishDetail.WpnStar4Rate = receiveRecordDao.CountRate(memberId, PoolType.武器, RareType.四星);
            wishDetail.StdStar4Rate = receiveRecordDao.CountRate(memberId, PoolType.常驻, RareType.四星);
            wishDetail.CharStar5Rate = receiveRecordDao.CountRate(memberId, PoolType.角色, RareType.五星);
            wishDetail.WpnStar5Rate = receiveRecordDao.CountRate(memberId, PoolType.武器, RareType.五星);
            wishDetail.StdStar5Rate = receiveRecordDao.CountRate(memberId, PoolType.常驻, RareType.五星);
            wishDetail.CharWishTimes = wishRecordDao.getWishTimes(memberId, PoolType.角色);
            wishDetail.WpnWishTimes = wishRecordDao.getWishTimes(memberId, PoolType.武器);
            wishDetail.StdWishTimes = wishRecordDao.getWishTimes(memberId, PoolType.常驻);
            wishDetail.TotalWishTimes = wishRecordDao.getWishTimes(memberId);
            return wishDetail;
        }

        public LuckRankingVO GetLuckRanking(int authId, int days, int top)
        {
            var luckRankingCache = DataCache.GetLuckRankingCache(authId);
            if (luckRankingCache != null) return luckRankingCache;
            DateTime startDate = DateTime.Now.AddDays(-1 * days);
            DateTime endDate = DateTime.Now;
            var star5RankingList = receiveRecordDao.GetLuckRanking(authId, RareType.五星, startDate, endDate, top);
            var star4RankingList = receiveRecordDao.GetLuckRanking(authId, RareType.四星, startDate, endDate, top);
            var luckRanking = new LuckRankingVO();
            luckRanking.Top = top;
            luckRanking.CountDay = days;
            luckRanking.StartTime = startDate.ToTimeStamp();
            luckRanking.EndTime = endDate.ToTimeStamp();
            luckRanking.Star5Ranking = star5RankingList.Select(m => ToRareRanking(m)).ToList();
            luckRanking.Star4Ranking = star4RankingList.Select(m => ToRareRanking(m)).ToList();
            DataCache.SetLuckRankingCache(authId, luckRanking);
            return luckRanking;
        }


        private RareRankingVO ToRareRanking(LuckRankingDto luckRankingDTO)
        {
            RareRankingVO rareRankingVO = new RareRankingVO();
            rareRankingVO.TotalWishTimes = luckRankingDTO.WishTimes;
            rareRankingVO.MemberCode = luckRankingDTO.MemberCode;
            rareRankingVO.MemberName = luckRankingDTO.MemberName;
            rareRankingVO.Count = luckRankingDTO.ReceiveCount;
            rareRankingVO.Rate = Math.Floor(luckRankingDTO.ReceiveRate * 100 * 10000) / 10000;
            return rareRankingVO;
        }

        /// <summary>
        /// 获取出货记录
        /// </summary>
        /// <param name="memberId"></param>
        /// <param name="rareType"></param>
        /// <param name="top"></param>
        /// <returns></returns>
        public List<ReceiveRecordDto> GetRecords(int memberId, RareType rareType, int top)
        {
            return receiveRecordDao.GetRecords(memberId, rareType, top);
        }

        /// <summary>
        /// 添加出货记录
        /// </summary>
        /// <param name="wishResult"></param>
        /// <param name="poolType"></param>
        /// <param name="memberId"></param>
        public void AddRecords(WishResultBO wishResult, PoolType poolType, int memberId)
        {
            var records = wishResult.WishRecords.Where(o => o.GoodsItem.RareType == RareType.五星 || o.GoodsItem.RareType == RareType.四星).ToList();
            foreach (var record in records) AddRecords(wishResult, record, poolType, memberId);
        }

        /// <summary>
        /// 添加出货记录
        /// </summary>
        /// <param name="wishResult"></param>
        /// <param name="wishRecord"></param>
        /// <param name="poolType"></param>
        /// <param name="memberId"></param>
        private void AddRecords(WishResultBO wishResult, WishRecordBO wishRecord, PoolType poolType, int memberId)
        {
            ReceiveRecordPO receiveRecord = new ReceiveRecordPO();
            receiveRecord.MemberId = memberId;
            receiveRecord.GoodsId = wishRecord.GoodsItem.GoodsID;
            receiveRecord.PoolType = poolType;
            receiveRecord.PoolIndex = wishResult.PoolIndex;
            receiveRecord.Cost = wishRecord.Cost;
            receiveRecord.CreateDate = DateTime.Now;
            receiveRecordDao.Insert(receiveRecord);
        }

    }
}
