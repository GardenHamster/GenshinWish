using System.Collections.Generic;

namespace GenshinWish.Models.BO
{
    public class UpItemBO
    {
        /// <summary>
        /// 5星列表
        /// </summary>
        public List<GoodsItemBO> Star5FullItems { get; set; }

        /// <summary>
        /// 4星列表
        /// </summary>
        public List<GoodsItemBO> Star4FullItems { get; set; }

        /// <summary>
        /// 3星列表
        /// </summary>
        public List<GoodsItemBO> Star3FullItems { get; set; }

        /// <summary>
        /// 5星角色up
        /// </summary>
        public List<GoodsItemBO> Star5UpItems { get; set; }

        /// <summary>
        /// 4星角色up
        /// </summary>
        public List<GoodsItemBO> Star4UpItems { get; set; }

        /// <summary>
        /// 5星角色非up
        /// </summary>
        public List<GoodsItemBO> Star5FixItems { get; set; }

        /// <summary>
        /// 4星角色非up
        /// </summary>
        public List<GoodsItemBO> Star4FixItems { get; set; }

        /// <summary>
        /// 蛋池编号
        /// </summary>
        public int PoolIndex { get; set; }


    }
}
