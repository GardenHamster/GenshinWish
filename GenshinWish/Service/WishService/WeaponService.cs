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
    public class WeaponService : BaseWishService
    {
        public WeaponService() { }

        public WeaponService(MemberDao memberDao, GoodsDao goodsDao) : base(memberDao, goodsDao)
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
        /// 十连保底物品概率
        /// </summary>
        protected readonly List<ProbabilityBO> Floor10List = new List<ProbabilityBO>()
        {
            new ProbabilityBO(0.7m, ProbabilityType.五星物品),
            new ProbabilityBO(99.3m,ProbabilityType.四星物品)
        };

        /// <summary>
        /// 小保底物品概率
        /// </summary>
        protected readonly List<ProbabilityBO> Floor80List = new List<ProbabilityBO>()
        {
            new ProbabilityBO(100, ProbabilityType.五星物品)
        };

        /// <summary>
        /// 获取祈愿结果
        /// </summary>
        /// <param name="authorize"></param>
        /// <param name="memberInfo"></param>
        /// <param name="upItem"></param>
        /// <param name="assignGoodsItem"></param>
        /// <param name="memberGoods"></param>
        /// <param name="wishCount"></param>
        /// <returns></returns>
        public WishResultBO GetWishResult(AuthorizePO authorize, MemberPO memberInfo, UpItemBO upItem, GoodsItemBO assignGoodsItem, List<MemberGoodsDto> memberGoods, int wishCount)
        {
            WishResultBO wishResult = new WishResultBO();
            WishRecordBO[] wishRecords = GetWishRecord(memberInfo, upItem, assignGoodsItem, memberGoods, wishCount);
            WishRecordBO[] sortRecords = SortRecords(wishRecords);
            wishResult.MemberInfo = memberInfo;
            wishResult.Authorize = authorize;
            wishResult.WishRecords = wishRecords;
            wishResult.SortWishRecords = sortRecords;
            wishResult.PoolIndex = upItem.PoolIndex;
            return wishResult;
        }

        /// <summary>
        /// 模拟抽卡,获取祈愿记录
        /// </summary>
        /// <param name="memberInfo"></param>
        /// <param name="upItem"></param>
        /// <param name="assignGoodsItem"></param>
        /// <param name="memberGoods"></param>
        /// <param name="wishCount">抽卡次数</param>
        /// <returns></returns>
        protected WishRecordBO[] GetWishRecord(MemberPO memberInfo, UpItemBO upItem, GoodsItemBO assignGoodsItem, List<MemberGoodsDto> memberGoods, int wishCount)
        {
            WishRecordBO[] records = new WishRecordBO[wishCount];
            for (int i = 0; i < records.Length; i++)
            {
                memberInfo.Wpn80Surplus--;
                memberInfo.Wpn20Surplus--;

                if (memberInfo.Wpn80Surplus < 14 && RandomHelper.getRandomBetween(1, 100) < (14 - memberInfo.Wpn80Surplus + 1) * 0.07 * 100)
                {
                    //武器池从第66抽开始,每抽出5星概率提高7%(基础概率),直到第80抽时概率上升到100%
                    records[i] = GetRandomItem(Floor80List, upItem, assignGoodsItem, memberInfo.AssignValue, memberInfo.Wpn20Surplus);
                }
                else if (memberInfo.Wpn20Surplus % 10 == 0)
                {
                    //十连保底
                    records[i] = GetRandomItem(Floor10List, upItem, assignGoodsItem, memberInfo.AssignValue, memberInfo.Wpn20Surplus);
                }
                else
                {
                    //无保底，无低保
                    records[i] = GetRandomItem(SingleList, upItem, assignGoodsItem, memberInfo.AssignValue, memberInfo.Wpn20Surplus);
                }

                bool isUpItem = IsUpItem(upItem, records[i].GoodsItem);//判断是否为本期up的物品
                bool isAssignItem = assignGoodsItem != null && records[i].GoodsItem.GoodsID == assignGoodsItem.GoodsID;//判断是否为本期定轨物品
                records[i].OwnedCount = GetOwnedCount(memberGoods, records, records[i]);//统计已拥有数量

                if (records[i].GoodsItem.RareType == RareType.四星 && isUpItem == false)
                {
                    records[i].Cost = 10 - memberInfo.Wpn20Surplus % 10;
                    memberInfo.Wpn20Surplus = 10;//十连保底重置
                }
                if (records[i].GoodsItem.RareType == RareType.四星 && isUpItem == true)
                {
                    records[i].Cost = 10 - memberInfo.Wpn20Surplus % 10;
                    memberInfo.Wpn20Surplus = 20;//十连保底重置
                }
                if (records[i].GoodsItem.RareType == RareType.五星 && isAssignItem == false)
                {
                    records[i].Cost = 80 - memberInfo.Wpn80Surplus;
                    if (assignGoodsItem != null) memberInfo.AssignValue++;//如果已经定轨，命定值+1
                    memberInfo.Wpn20Surplus = 20;//十连大保底重置
                    memberInfo.Wpn80Surplus = 80;//八十发保底重置
                }
                if (records[i].GoodsItem.RareType == RareType.五星 && isAssignItem == true)
                {
                    records[i].Cost = 80 - memberInfo.Wpn80Surplus;
                    if (assignGoodsItem != null) memberInfo.AssignValue = 0;//命定值重置
                    memberInfo.Wpn20Surplus = 20;//十连大保底重置
                    memberInfo.Wpn80Surplus = 80;//八十发保底重置
                }
                //当命定值溢出或者定轨项目不在本期5星UP范围内时，重置命定值
                if (assignGoodsItem == null || memberInfo.AssignValue > 2)
                {
                    memberInfo.AssignId = 0;
                    memberInfo.AssignValue = 0;
                }
            }

            return records;
        }

        protected WishRecordBO GetRandomItem(List<ProbabilityBO> probabilities, UpItemBO ysUpItem, GoodsItemBO assignGoodsItem, int assignValue, int floor20Surplus)
        {
            ProbabilityBO ysProbability = GetRandomInList(probabilities);
            if (ysProbability.ProbabilityType == ProbabilityType.五星物品)
            {
                //命定值达到满值后，在本次祈愿中获得的下一把5星武器，必定为当前定轨武器
                bool isGetAssign = assignGoodsItem != null && assignValue >= 2;
                if (isGetAssign) return new WishRecordBO(assignGoodsItem);
                //当祈愿获取到5星武器时，有75.000%的概率为本期5星UP武器
                bool isGetUp = RandomHelper.getRandomBetween(1, 100) <= 75;
                return isGetUp ? GetRandomInList(ysUpItem.Star5UpList) : GetRandomInList(ysUpItem.Star5NonUpList);
            }
            if (ysProbability.ProbabilityType == ProbabilityType.四星物品)
            {
                //当祈愿获取到4星物品时，有75.000%的概率为本期4星UP武器
                bool isGetUp = floor20Surplus < 10 ? true : RandomHelper.getRandomBetween(1, 100) <= 75;
                return isGetUp ? GetRandomInList(ysUpItem.Star4UpList) : GetRandomInList(ysUpItem.Star4NonUpList);
            }
            if (ysProbability.ProbabilityType == ProbabilityType.三星物品)
            {
                return GetRandomInList(ysUpItem.Star3AllList);
            }
            throw new GoodsNotFoundException($"未能随机获取与{Enum.GetName(typeof(ProbabilityBO), ysProbability.ProbabilityType)}对应物品");
        }

    }
}
