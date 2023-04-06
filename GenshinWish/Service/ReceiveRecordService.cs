using GenshinWish.Common;
using GenshinWish.Dao;
using GenshinWish.Models.DTO;
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
            wishDetail.CharStar4Rate = receiveRecordDao.CountRate(memberId, WishType.角色, RareType.四星);
            wishDetail.WpnStar4Rate = receiveRecordDao.CountRate(memberId, WishType.武器, RareType.四星);
            wishDetail.StdStar4Rate = receiveRecordDao.CountRate(memberId, WishType.常驻, RareType.四星);
            wishDetail.CharStar5Rate = receiveRecordDao.CountRate(memberId, WishType.角色, RareType.五星);
            wishDetail.WpnStar5Rate = receiveRecordDao.CountRate(memberId, WishType.武器, RareType.五星);
            wishDetail.StdStar5Rate = receiveRecordDao.CountRate(memberId, WishType.常驻, RareType.五星);
            wishDetail.CharWishTimes = wishRecordDao.getWishTimes(memberId, WishType.角色);
            wishDetail.WpnWishTimes = wishRecordDao.getWishTimes(memberId, WishType.武器);
            wishDetail.StdWishTimes = wishRecordDao.getWishTimes(memberId, WishType.常驻);
            wishDetail.TotalWishTimes = wishRecordDao.getWishTimes(memberId);
            return wishDetail;
        }

        public LuckRankingVO getLuckRanking(int authId, int days, int top)
        {
            LuckRankingVO luckRankingCache = DataCache.GetLuckRankingCache(authId);
            if (luckRankingCache != null) return luckRankingCache;
            DateTime startDate = DateTime.Now.AddDays(-1 * days);
            DateTime endDate = DateTime.Now;
            var star5RankingList = receiveRecordDao.GetLuckRanking(authId, RareType.五星, startDate, endDate, top);
            var star4RankingList = receiveRecordDao.GetLuckRanking(authId, RareType.四星, startDate, endDate, top);
            var luckRanking = new LuckRankingVO();
            luckRanking.Top = top;
            luckRanking.CountDay = days;
            luckRanking.StartDate = startDate;
            luckRanking.EndDate = endDate;
            luckRanking.Star5Ranking = star5RankingList.Select(m => toRareRanking(m)).ToList();
            luckRanking.Star4Ranking = star4RankingList.Select(m => toRareRanking(m)).ToList();
            DataCache.SetLuckRankingCache(authId, luckRanking);
            return luckRanking;
        }

        private RareRankingVO toRareRanking(LuckRankingDto luckRankingDTO)
        {
            RareRankingVO rareRankingVO = new RareRankingVO();
            rareRankingVO.TotalWishTimes = luckRankingDTO.TotalWishTimes;
            rareRankingVO.MemberCode = luckRankingDTO.MemberCode;
            rareRankingVO.MemberName = luckRankingDTO.MemberName;
            rareRankingVO.Count = luckRankingDTO.RareCount;
            rareRankingVO.Rate = Math.Floor(luckRankingDTO.RareRate * 100 * 1000) / 1000;
            return rareRankingVO;
        }

        public List<ReceiveRecordDto> getRecords(int memberId, RareType rareType, int top)
        {
            return receiveRecordDao.GetRecords(memberId, rareType, top);
        }

    }
}
