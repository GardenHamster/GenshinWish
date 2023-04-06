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
    public class CharacterService : BaseWishService
    {
        public CharacterService() { }

        public CharacterService(MemberDao memberDao, GoodsDao goodsDao) : base(memberDao, goodsDao)
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
                memberInfo.Char180Surplus--;
                memberInfo.Char20Surplus--;

                if (memberInfo.Char180Surplus % 90 < 16 && RandomHelper.getRandomBetween(1, 100) < (16 - memberInfo.Char180Surplus % 90 + 1) * 0.06 * 100)//低保
                {
                    //角色池从第74抽开始,每抽出5星概率提高6%(基础概率),直到第90抽时概率上升到100%
                    record = GetRandomItem(Floor90List, upItem, memberInfo.Char180Surplus, memberInfo.Char20Surplus);
                }
                else if (memberInfo.Char20Surplus % 10 == 0)
                {
                    record = GetRandomItem(SingleList, upItem, memberInfo.Char180Surplus, memberInfo.Char20Surplus);//十连保底
                }
                else
                {
                    record = GetRandomItem(Floor10List, upItem, memberInfo.Char180Surplus, memberInfo.Char20Surplus);//无保底，无低保
                }
                
                bool isUpItem = IsUpItem(upItem, record.GoodsItem);//判断是否为本期up的物品
                
                if (record.GoodsItem.RareType == RareType.四星 && isUpItem == false)
                {
                    record.Cost = 10 - memberInfo.Char20Surplus % 10;
                    memberInfo.Char20Surplus = 10;//十连大保底重置为10
                }
                if (record.GoodsItem.RareType == RareType.四星 && isUpItem == true)
                {
                    record.Cost = 10 - memberInfo.Char20Surplus % 10;
                    memberInfo.Char20Surplus = 20;//十连大保底重置
                }
                if (record.GoodsItem.RareType == RareType.五星 && isUpItem == false)
                {
                    record.Cost = 90 - memberInfo.Char180Surplus % 90;
                    memberInfo.Char20Surplus = 20;//十连大保底重置
                    memberInfo.Char180Surplus = 90;//九十发大保底重置为90
                }
                if (record.GoodsItem.RareType == RareType.五星 && isUpItem == true)
                {
                    record.Cost = 90 - memberInfo.Char180Surplus % 90;
                    memberInfo.Char20Surplus = 20;//十连大保底重置
                    memberInfo.Char180Surplus = 180;//九十发大保底重置
                }

                record.OwnedCount = GetOwnedCount(memberGoods, records, record);//统计已拥有数量
                records[i] = record;
            }
            return records;
        }

        protected WishRecordBO GetRandomItem(List<ProbabilityBO> probabilities, UpItemBO ySUpItem, int floor180Surplus, int floor20Surplus)
        {
            ProbabilityBO ysProbability = GetRandomInList(probabilities);
            if (ysProbability.ProbabilityType == ProbabilityType.五星物品)
            {
                //当祈愿获取到5星角色时，有50.000%的概率为本期5星UP角色
                bool isGetUp = floor180Surplus < 90 ? true : RandomHelper.getRandomBetween(1, 100) <= 50;
                return isGetUp ? GetRandomInList(ySUpItem.Star5UpList) : GetRandomInList(ySUpItem.Star5NonUpList);
            }
            if (ysProbability.ProbabilityType == ProbabilityType.四星物品)
            {
                //当祈愿获取到4星物品时，有50.000%的概率为本期4星UP角色
                bool isGetUp = floor20Surplus < 10 ? true : RandomHelper.getRandomBetween(1, 100) <= 50;
                return isGetUp ? GetRandomInList(ySUpItem.Star4UpList) : GetRandomInList(ySUpItem.Star4NonUpList);
            }
            if (ysProbability.ProbabilityType == ProbabilityType.三星物品)
            {
                return GetRandomInList(ySUpItem.Star3AllList);
            }
            throw new GoodsNotFoundException($"未能随机获取与{Enum.GetName(typeof(ProbabilityBO), ysProbability.ProbabilityType)}对应物品");
        }

    }
}
