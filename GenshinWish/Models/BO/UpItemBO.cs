using System.Collections.Generic;

namespace GenshinWish.Models.BO
{
    public class UpItemBO
    {
        /// <summary>
        /// 5星列表
        /// </summary>
        public List<GoodsItemBO> Star5AllList { get; set; }

        /// <summary>
        /// 4星列表
        /// </summary>
        public List<GoodsItemBO> Star4AllList { get; set; }

        /// <summary>
        /// 3星列表
        /// </summary>
        public List<GoodsItemBO> Star3AllList { get; set; }

        /// <summary>
        /// 5星角色up
        /// </summary>
        public List<GoodsItemBO> Star5UpList { get; set; }

        /// <summary>
        /// 4星角色up
        /// </summary>
        public List<GoodsItemBO> Star4UpList { get; set; }

        /// <summary>
        /// 5星角色非up
        /// </summary>
        public List<GoodsItemBO> Star5NonUpList { get; set; }

        /// <summary>
        /// 4星角色非up
        /// </summary>
        public List<GoodsItemBO> Star4NonUpList { get; set; }

        /// <summary>
        /// 蛋池编号
        /// </summary>
        public int PoolIndex { get; set; }


    }
}
