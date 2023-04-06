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
    public class StandardService : BaseWishService
    {
        public StandardService() { }

        public StandardService(MemberDao memberDao, GoodsDao goodsDao) : base(memberDao, goodsDao)
        {
            this.memberDao = memberDao;
            this.goodsDao = goodsDao;
        }

        /// <summary>
        /// 无保底情况下单抽物品概率
        /// </summary>
        protected readonly List<ProbabilityBO> SingleList = new List<ProbabilityBO>()
        {
            new ProbabilityBO(0.6m, ProbabilityType.五星物品),
            new ProbabilityBO(5.1m, ProbabilityType.四星物品),
            new ProbabilityBO(94.3m,ProbabilityType.三星物品)
        };

        /// <summary>
        /// 小保底物品概率
        /// </summary>
        protected readonly List<ProbabilityBO> Floor90List = new List<ProbabilityBO>()
        {
            new ProbabilityBO(100, ProbabilityType.五星物品),
        };

        /// <summary>
        /// 十连保底物品概率
        /// </summary>
        protected readonly List<ProbabilityBO> Floor10List = new List<ProbabilityBO>()
        {
            new ProbabilityBO(0.6m, ProbabilityType.五星物品),
            new ProbabilityBO(99.4m,ProbabilityType.四星物品)
        };


        /// <summary>
        /// 模拟抽卡,获取祈愿记录
        /// </summary>
        /// <param name="memberInfo"></param>
        /// <param name="ySUpItem"></param>
        /// <param name="memberGoods"></param>
        /// <param name="wishCount">抽卡次数</param>
        /// <returns></returns>
        public virtual WishRecordBO[] GetWishRecord(MemberPO memberInfo, UpItemBO ySUpItem, List<MemberGoodsDto> memberGoods, int wishCount)
        {
            WishRecordBO[] records = new WishRecordBO[wishCount];
            for (int i = 0; i < records.Length; i++)
            {
                memberInfo.Std90Surplus--;
                memberInfo.Std10Surplus--;

                if (memberInfo.Std10Surplus > 0)//无保底
                {
                    records[i] = GetRandomItem(SingleList, ySUpItem);
                }
                if (memberInfo.Std10Surplus <= 0)//十连保底
                {
                    records[i] = GetRandomItem(Floor10List, ySUpItem);
                }
                //常驻池从第74抽开始,每抽出5星概率提高6%(基础概率),直到第90抽时概率上升到100%
                if (memberInfo.Std90Surplus < 16 && RandomHelper.getRandomBetween(1, 100) < (16 - memberInfo.Std90Surplus + 1) * 0.06 * 100)//低保
                {
                    records[i] = GetRandomItem(Floor90List, ySUpItem);
                }

                records[i].OwnedCount = GetOwnedCount(memberGoods, records, records[i]);//统计已拥有数量

                if (records[i].GoodsItem.RareType == RareType.四星)
                {
                    records[i].Cost = 10 - memberInfo.Std10Surplus;
                    memberInfo.Std10Surplus = 10;//十连保底重置
                }
                if (records[i].GoodsItem.RareType == RareType.五星)
                {
                    records[i].Cost = 90 - memberInfo.Std90Surplus;
                    memberInfo.Std10Surplus = 10;//十连保底重置
                    memberInfo.Std90Surplus = 90;//九十发保底重置
                }
            }
            return records;
        }


        protected WishRecordBO GetRandomItem(List<ProbabilityBO> probabilities, UpItemBO ySUpItem)
        {
            ProbabilityBO ysProbability = GetRandomInList(probabilities);
            if (ysProbability.ProbabilityType == ProbabilityType.五星物品)
            {
                return GetRandomInList(ySUpItem.Star5AllList);
            }
            if (ysProbability.ProbabilityType == ProbabilityType.四星物品)
            {
                return GetRandomInList(ySUpItem.Star4AllList);
            }
            if (ysProbability.ProbabilityType == ProbabilityType.三星物品)
            {
                return GetRandomInList(ySUpItem.Star3AllList);
            }
            throw new GoodsNotFoundException($"未能随机获取与{Enum.GetName(typeof(ProbabilityBO), ysProbability.ProbabilityType)}对应物品");
        }

        public WishResultBO GetWishResult(AuthorizePO authorize, MemberPO memberInfo, UpItemBO ysUpItem, List<MemberGoodsDto> memberGoods, int wishCount)
        {
            WishResultBO wishResult = new WishResultBO();
            int perm90SurplusBefore = memberInfo.Std90Surplus;

            WishRecordBO[] wishRecords = GetWishRecord(memberInfo, ysUpItem, memberGoods, wishCount);
            WishRecordBO[] sortRecords = SortRecords(wishRecords);

            wishResult.MemberInfo = memberInfo;
            wishResult.Authorize = authorize;
            wishResult.WishRecords = wishRecords;
            wishResult.SortWishRecords = sortRecords;
            wishResult.Star5Cost = GetStar5Cost(wishRecords, perm90SurplusBefore, 90);
            wishResult.Surplus10 = memberInfo.Std10Surplus;
            return wishResult;
        }

    }
}
