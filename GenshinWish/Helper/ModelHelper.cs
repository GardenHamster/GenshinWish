using GenshinWish.Models.BO;
using GenshinWish.Models.DTO;
using GenshinWish.Models.VO;
using GenshinWish.Type;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GenshinWish.Helper
{
    public static class ModelHelper
    {
        /// <summary>
        /// 转换为WishRecordVO
        /// </summary>
        /// <param name="recordList"></param>
        /// <returns></returns>
        public static List<WishRecordVO> ToWishRecordVO(this List<ReceiveRecordDto> recordList)
        {
            return recordList.Select(m => new WishRecordVO()
            {
                GoodsName = m.GoodsName,
                GoodsType = Enum.GetName(typeof(GoodsType), m.GoodsType),
                GoodsSubType = Enum.GetName(typeof(GoodsSubType), m.GoodsSubType),
                RareType = Enum.GetName(typeof(RareType), m.RareType),
                PoolType = Enum.GetName(typeof(PoolType), m.PoolType),
                Cost = m.Cost,
                CreateDate = m.CreateDate
            }).ToList();
        }

        /// <summary>
        /// 转换为GoodsVO
        /// </summary>
        /// <param name="wishRecords"></param>
        /// <returns></returns>
        public static List<GoodsVO> ToGoodsVO(this WishRecordBO[] wishRecords)
        {
            return wishRecords.Select(m => new GoodsVO()
            {
                Cost = m.Cost,
                GoodsName = m.GoodsItem.GoodsName,
                GoodsType = Enum.GetName(typeof(GoodsType), m.GoodsItem.GoodsType),
                GoodsSubType = Enum.GetName(typeof(GoodsSubType), m.GoodsItem.GoodsSubType),
                RareType = Enum.GetName(typeof(RareType), m.GoodsItem.RareType),
            }).ToList();
        }

        /// <summary>
        /// 转换为GoodsVO
        /// </summary>
        /// <param name="goodsItems"></param>
        /// <returns></returns>
        public static List<GoodsVO> ToGoodsVO(this List<GoodsItemBO> goodsItems)
        {
            return goodsItems.Select(m => new GoodsVO()
            {
                GoodsName = m.GoodsName,
                GoodsType = Enum.GetName(typeof(GoodsType), m.GoodsType),
                GoodsSubType = Enum.GetName(typeof(GoodsSubType), m.GoodsSubType),
                RareType = Enum.GetName(typeof(RareType), m.RareType)
            }).ToList();
        }

    }
}
