using GenshinWish.Helper;
using GenshinWish.Models.BO;
using GenshinWish.Models.DTO;
using GenshinWish.Type;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace GenshinWish.Service.WishService
{
    public abstract class BaseWishService : BaseService
    {
        /// <summary>
        /// 显示顺序排序
        /// </summary>
        /// <param name="records"></param>
        /// <returns></returns>
        public WishRecordBO[] SortRecords(WishRecordBO[] records)
        {
            //先按稀有度倒序排序（5->1），然后按角色->武器种类排序（0->2），最后New排在前面
            return records.OrderByDescending(c => c.GoodsItem.RareType).ThenBy(c => c.GoodsItem.GoodsType).ThenBy(c => c.OwnedCount).ToArray();
        }

        /// <summary>
        /// 返回前10条无重复的结果，5星记录除外
        /// </summary>
        /// <param name="records"></param>
        /// <returns></returns>
        public WishRecordBO[] FilterRecords(WishRecordBO[] records)
        {
            if (records.Length <= 10) return records;
            var star5Records = records.Where(o => o.GoodsItem.RareType == RareType.五星);
            var star4Records = records.Where(o => o.GoodsItem.RareType == RareType.四星).GroupBy(o => o.GoodsItem.GoodsID).Select(o => o.OrderByDescending(m => m.OwnedCount).First());
            var star3Records = records.Where(o => o.GoodsItem.RareType == RareType.三星).GroupBy(o => o.GoodsItem.GoodsID).Select(o => o.OrderByDescending(m => m.OwnedCount).First());
            return star5Records.Concat(star4Records).Concat(star3Records).Take(10).ToArray();
        }

        /// <summary>
        /// 从物品列表中随机出一个物品
        /// </summary>
        /// <param name="probabilityList"></param>
        /// <returns></returns>
        protected ProbabilityBO GetRandomInList(List<ProbabilityBO> probabilityList)
        {
            List<RegionBO<ProbabilityBO>> regionList = GetRegionList(probabilityList);
            RegionBO<ProbabilityBO> region = GetRandomInRegion(regionList);
            return region.Item;
        }

        /// <summary>
        /// 从物品列表中随机出一个物品
        /// </summary>
        /// <param name="goodsItemList"></param>
        /// <returns></returns>
        protected WishRecordBO GetRandomInList(List<GoodsItemBO> goodsItemList)
        {
            int randomIndex = RandomHelper.getRandomBetween(0, goodsItemList.Count - 1);
            return new WishRecordBO(goodsItemList[randomIndex]);
        }

        /// <summary>
        /// 将概率转化为一个数字区间
        /// </summary>
        /// <param name="probabilityList"></param>
        /// <returns></returns>
        private List<RegionBO<ProbabilityBO>> GetRegionList(List<ProbabilityBO> probabilityList)
        {
            int sumRegion = 0;//总区间
            List<RegionBO<ProbabilityBO>> regionList = new List<RegionBO<ProbabilityBO>>();//区间列表,抽卡时随机获取该区间
            foreach (var item in probabilityList)
            {
                int startRegion = sumRegion;//开始区间
                sumRegion = startRegion + Convert.ToInt32(item.Probability * 10000);//结束区间
                regionList.Add(new RegionBO<ProbabilityBO>(item, startRegion, sumRegion));
            }
            return regionList;
        }

        /// <summary>
        /// 从区间列表中随机出一个区间
        /// </summary>
        /// <param name="regionList"></param>
        /// <returns></returns>
        private RegionBO<ProbabilityBO> GetRandomInRegion(List<RegionBO<ProbabilityBO>> regionList)
        {
            int randomRegion = RandomHelper.getRandomBetween(0, regionList.Last().EndRegion);
            foreach (var item in regionList)
            {
                if (randomRegion >= item.StartRegion && randomRegion < item.EndRegion) return item;
            }
            return regionList.Last();
        }

        /// <summary>
        /// 判断一个项目是否up项目
        /// </summary> 
        /// <param name="upItem"></param>
        /// <param name="goodsItem"></param>
        /// <returns></returns>
        protected bool IsUpItem(UpItemBO upItem, GoodsItemBO goodsItem)
        {
            if (upItem.Star5UpItems.Where(m => m.GoodsName == goodsItem.GoodsName).Count() > 0) return true;
            if (upItem.Star4UpItems.Where(m => m.GoodsName == goodsItem.GoodsName).Count() > 0) return true;
            return false;
        }

        /// <summary>
        /// 获取一次五星保底内,成员获得5星角色的累计祈愿次数,0代表还未获得S
        /// </summary>
        /// <param name="wishRecords">祈愿结果</param>
        /// <param name="floorSurplus">剩余N次保底</param>
        /// <param name="maxSurplus">抽出5星最多需要N抽</param>
        /// <returns></returns>
        public int GetStar5Cost(WishRecordBO[] wishRecords, int floorSurplus, int maxSurplus)
        {
            int star5Index = -1;
            for (int i = 0; i < wishRecords.Length; i++)
            {
                GoodsItemBO GoodsItem = wishRecords[i].GoodsItem;
                if (GoodsItem.RareType != RareType.五星) continue;
                star5Index = i;
                break;
            }
            if (star5Index == -1) return 0;
            return maxSurplus - floorSurplus + star5Index + 1;
        }

        /// <summary>
        /// 获取一个物品的当前已拥有数量
        /// </summary>
        /// <param name="memberGoods"></param>
        /// <param name="records"></param>
        /// <param name="checkRecord"></param>
        /// <returns></returns>
        protected int GetOwnedCount(List<MemberGoodsBO> memberGoods, WishRecordBO[] records, WishRecordBO checkRecord)
        {
            MemberGoodsBO ownedGood = memberGoods.Where(m => m.GoodsId == checkRecord.GoodsItem.GoodsID).FirstOrDefault();
            int ownInDatabase = ownedGood == null ? 0 : ownedGood.Count;
            int ownInRecord = records.Where(m => m is not null && m.GoodsItem.GoodsID == checkRecord.GoodsItem.GoodsID).Count();
            return ownInDatabase + ownInRecord;
        }

    }
}
