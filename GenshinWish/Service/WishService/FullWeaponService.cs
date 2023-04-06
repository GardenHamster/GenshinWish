using GenshinWish.Dao;
using GenshinWish.Exceptions;
using GenshinWish.Models.BO;
using GenshinWish.Models.DTO;
using GenshinWish.Models.PO;
using GenshinWish.Type;
using GenshinWish.Util;
using System;
using System.Collections.Generic;

namespace GenshinWish.Service.WishService
{
    public class FullWeaponService : BaseWishService
    {
        public FullWeaponService() { }

        public FullWeaponService(MemberDao memberDao, GoodsDao goodsDao) : base(memberDao, goodsDao)
        {
            this.memberDao = memberDao;
            this.goodsDao = goodsDao;
        }

        /// <summary>
        /// 无保底情况下单抽物品概率
        /// </summary>
        protected readonly List<ProbabilityBO> SingleList = new List<ProbabilityBO>()
        {
            new ProbabilityBO(0.7m, ProbabilityType.五星物品),
            new ProbabilityBO(6.0m, ProbabilityType.四星物品),
            new ProbabilityBO(93.3m,ProbabilityType.三星物品)
        };

        /// <summary>
        /// 小保底物品概率
        /// </summary>
        protected readonly List<ProbabilityBO> Floor80List = new List<ProbabilityBO>()
        {
            new ProbabilityBO(100, ProbabilityType.五星物品)
        };

        /// <summary>
        /// 十连保底物品概率
        /// </summary>
        protected readonly List<ProbabilityBO> Floor10List = new List<ProbabilityBO>()
        {
            new ProbabilityBO(0.7m, ProbabilityType.五星物品),
            new ProbabilityBO(99.3m,ProbabilityType.四星物品)
        };

        /// <summary>
        /// 获取祈愿结果
        /// </summary>
        /// <param name="authorize"></param>
        /// <param name="memberInfo"></param>
        /// <param name="upItem"></param>
        /// <param name="memberGoods"></param>
        /// <param name="wishCount"></param>
        /// <returns></returns>
        public WishResultBO GetWishResult(AuthorizePO authorize, MemberPO memberInfo, UpItemBO upItem, List<MemberGoodsDto> memberGoods, int wishCount)
        {
            WishRecordBO[] wishRecords = GetWishRecord(memberInfo, upItem, memberGoods, wishCount);
            WishRecordBO[] sortRecords = SortRecords(wishRecords);
            WishResultBO wishResult = new WishResultBO();
            wishResult.MemberInfo = memberInfo;
            wishResult.Authorize = authorize;
            wishResult.WishRecords = wishRecords;
            wishResult.SortWishRecords = sortRecords;
            wishResult.PoolIndex = upItem.PoolIndex;
            return wishResult;
        }

        /// <summary>
        /// 获取祈愿记录
        /// </summary>
        /// <param name="memberInfo"></param>
        /// <param name="upItem"></param>
        /// <param name="memberGoods"></param>
        /// <param name="wishCount">抽卡次数</param>
        /// <returns></returns>
        public virtual WishRecordBO[] GetWishRecord(MemberPO memberInfo, UpItemBO upItem, List<MemberGoodsDto> memberGoods, int wishCount)
        {
            WishRecordBO[] records = new WishRecordBO[wishCount];
            for (int i = 0; i < records.Length; i++)
            {
                memberInfo.FullWpn80Surplus--;
                memberInfo.FullWpn10Surplus--;

                if (memberInfo.FullWpn80Surplus < 14 && RandomHelper.getRandomBetween(1, 100) < (14 - memberInfo.FullWpn80Surplus + 1) * 0.07 * 100)//低保
                {
                    //武器池从第66抽开始,每抽出5星概率提高7%(基础概率),直到第80抽时概率上升到100%
                    records[i] = GetRandomItem(Floor80List, upItem);
                }
                else if (memberInfo.FullWpn10Surplus % 10 == 0)
                {
                    //十连保底
                    records[i] = GetRandomItem(SingleList, upItem);
                }
                else
                {
                    //无保底，无低保
                    records[i] = GetRandomItem(Floor10List, upItem);
                }

                records[i].OwnedCount = GetOwnedCount(memberGoods, records, records[i]);//统计已拥有数量

                if (records[i].GoodsItem.RareType == RareType.四星)
                {
                    records[i].Cost = 10 - memberInfo.FullWpn10Surplus;
                    memberInfo.FullWpn10Surplus = 10;//十连小保底重置
                }
                if (records[i].GoodsItem.RareType == RareType.五星)
                {
                    records[i].Cost = 80 - memberInfo.FullWpn80Surplus;
                    memberInfo.FullWpn10Surplus = 10;//十连小保底重置
                    memberInfo.FullWpn80Surplus = 80;//八十发保底重置
                }
            }
            return records;
        }

        protected WishRecordBO GetRandomItem(List<ProbabilityBO> probabilities, UpItemBO ysUpItem)
        {
            ProbabilityBO ysProbability = GetRandomInList(probabilities);
            if (ysProbability.ProbabilityType == ProbabilityType.五星物品) return GetRandomInList(ysUpItem.Star5AllList);
            if (ysProbability.ProbabilityType == ProbabilityType.四星物品) return GetRandomInList(ysUpItem.Star4AllList);
            if (ysProbability.ProbabilityType == ProbabilityType.三星物品) return GetRandomInList(ysUpItem.Star3AllList);
            throw new GoodsNotFoundException($"未能随机获取与{Enum.GetName(typeof(ProbabilityBO), ysProbability.ProbabilityType)}类型对应的物品");
        }

    }
}
