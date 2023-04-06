using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GenshinWish.Models.BO
{
    public class WishRecordBO
    {
        /// <summary>
        /// 获得补给项目
        /// </summary>
        public GoodsItemBO GoodsItem { get; set; }

        /// <summary>
        /// 包括当前结果在内,当前结果以前所拥有的数量
        /// </summary>
        public int OwnedCount { get; set; }

        /// <summary>
        /// 在一次保底中消耗多少抽
        /// </summary>
        public int Cost { get; set; }

        public WishRecordBO(GoodsItemBO goodsItem)
        {
            OwnedCount = 1;
            GoodsItem = goodsItem;
        }

        public WishRecordBO(GoodsItemBO goodsItem, int ownedCount, int cost)
        {
            GoodsItem = goodsItem;
            OwnedCount = ownedCount;
            Cost = cost;
        }


    }
}
