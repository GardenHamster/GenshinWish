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
                WishRecordBO record;
                memberInfo.Std90Surplus--;
                memberInfo.Std10Surplus--;

                if (memberInfo.Std90Surplus < 16 && RandomHelper.getRandomBetween(1, 100) < (16 - memberInfo.Std90Surplus + 1) * 0.06 * 100)//低保
                {
                    //常驻池从第74抽开始,每抽出5星概率提高6%(基础概率),直到第90抽时概率上升到100%
                    record = GetRandomItem(Floor90List, upItem);
                }
                else if (memberInfo.Std10Surplus % 10 == 0)
                {
                    record = GetRandomItem(SingleList, upItem);//十连保底
                }
                else
                {
                    record = GetRandomItem(Floor10List, upItem);//无保底，无低保
                }

                if (record.GoodsItem.RareType == RareType.四星)
                {
                    record.Cost = 10 - memberInfo.Std10Surplus;
                    memberInfo.Std10Surplus = 10;//十连保底重置
                }
                if (record.GoodsItem.RareType == RareType.五星)
                {
                    record.Cost = 90 - memberInfo.Std90Surplus;
                    memberInfo.Std10Surplus = 10;//十连保底重置
                    memberInfo.Std90Surplus = 90;//九十发保底重置
                }

                record.OwnedCount = GetOwnedCount(memberGoods, records, record);//统计已拥有数量
                records[i] = record;
            }
            return records;
        }

        protected WishRecordBO GetRandomItem(List<ProbabilityBO> probabilities, UpItemBO ySUpItem)
        {
            ProbabilityBO ysProbability = GetRandomInList(probabilities);
            if (ysProbability.ProbabilityType == ProbabilityType.五星物品) return GetRandomInList(ySUpItem.Star5AllList);
            if (ysProbability.ProbabilityType == ProbabilityType.四星物品) return GetRandomInList(ySUpItem.Star4AllList);
            if (ysProbability.ProbabilityType == ProbabilityType.三星物品) return GetRandomInList(ySUpItem.Star3AllList);
            throw new GoodsNotFoundException($"未能随机获取与{Enum.GetName(typeof(ProbabilityBO), ysProbability.ProbabilityType)}对应物品");
        }

    }
}
