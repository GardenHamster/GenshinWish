using GenshinWish.Type;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GenshinWish.Models.BO
{
    public record WishRecordBO
    {
        /// <summary>
        /// 获得的物品
        /// </summary>
        public GoodsItemBO GoodsItem { get; set; }

        /// <summary>
        /// 包括当前结果在内，已拥有该物品的数量
        /// </summary>
        public int OwnedCount { get; set; }

        /// <summary>
        /// 在一次保底中消耗多少抽
        /// </summary>
        public int Cost { get; set; }

        public WishRecordBO(GoodsItemBO goodsItem)
        {
            this.Cost = 1;
            this.OwnedCount = 1;
            this.GoodsItem = goodsItem;
        }

        public WishRecordBO(GoodsItemBO goodsItem, int ownedCount, int cost)
        {
            this.Cost = cost;
            this.OwnedCount = ownedCount;
            this.GoodsItem = goodsItem;
        }


    }
}
