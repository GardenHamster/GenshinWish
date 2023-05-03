using GenshinWish.Dao;
using GenshinWish.Exceptions;
using GenshinWish.Helper;
using GenshinWish.Models.BO;
using GenshinWish.Models.PO;
using GenshinWish.Type;
using System;
using System.Collections.Generic;

namespace GenshinWish.Service.WishService
{
    public class FullCharacterService : BaseWishService
    {
        protected GoodsDao goodsDao;
        protected MemberDao memberDao;

        public FullCharacterService() { }

        public FullCharacterService(MemberDao memberDao, GoodsDao goodsDao)
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
        public WishResultBO GetWishResult(AuthorizePO authorize, MemberPO memberInfo, UpItemBO upItem, List<MemberGoodsBO> memberGoods, int wishCount)
        {
            WishRecordBO[] wishRecords = GetWishRecord(memberInfo, upItem, memberGoods, wishCount);
            WishRecordBO[] filterRecords = FilterRecords(wishRecords);
            WishRecordBO[] sortRecords = SortRecords(filterRecords);
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
        public virtual WishRecordBO[] GetWishRecord(MemberPO memberInfo, UpItemBO upItem, List<MemberGoodsBO> memberGoods, int wishCount)
        {
            WishRecordBO[] records = new WishRecordBO[wishCount];
            for (int i = 0; i < records.Length; i++)
            {
                WishRecordBO record;
                memberInfo.FullChar90Surplus--;
                memberInfo.FullChar10Surplus--;

                if (memberInfo.FullChar90Surplus < 16 && RandomHelper.getRandomBetween(1, 100) < (16 - memberInfo.FullChar90Surplus + 1) * 0.06 * 100)//低保
                {
                    //角色池从第74抽开始,每抽出5星概率提高6%(基础概率),直到第90抽时概率上升到100%
                    record = GetRandomItem(Floor90List, upItem);
                }
                else if (memberInfo.FullChar10Surplus % 10 == 0)
                {
                    record = GetRandomItem(Floor10List, upItem);//十连保底
                }
                else
                {
                    record = GetRandomItem(SingleList, upItem);//无保底，无低保
                }

                if (record.GoodsItem.RareType == RareType.四星)
                {
                    record.Cost = 10 - memberInfo.FullChar10Surplus;
                    memberInfo.FullChar10Surplus = 10;//十连小保底重置
                }
                if (record.GoodsItem.RareType == RareType.五星)
                {
                    record.Cost = 90 - memberInfo.FullChar90Surplus;
                    memberInfo.FullChar10Surplus = 10;//十连小保底重置
                    memberInfo.FullChar90Surplus = 90;//九十发小保底重置
                }

                records[i] = record;
                records[i].OwnedCount = GetOwnedCount(memberGoods, records, record);//统计已拥有数量
            }
            return records;
        }

        protected WishRecordBO GetRandomItem(List<ProbabilityBO> probabilities, UpItemBO upItem)
        {
            ProbabilityBO ysProbability = GetRandomInList(probabilities);
            if (ysProbability.ProbabilityType == ProbabilityType.五星物品) return GetRandomInList(upItem.Star5FullItems);
            if (ysProbability.ProbabilityType == ProbabilityType.四星物品) return GetRandomInList(upItem.Star4FullItems);
            if (ysProbability.ProbabilityType == ProbabilityType.三星物品) return GetRandomInList(upItem.Star3FullItems);
            throw new GoodsNotFoundException($"未能随机获取与{Enum.GetName(typeof(ProbabilityBO), ysProbability.ProbabilityType)}类型对应的物品");
        }

    }
}
