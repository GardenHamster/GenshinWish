using System.Collections.Generic;

namespace GenshinWish.Models.DTO
{
    public class GenerateDataDto
    {
        /// <summary>
        /// 物品列表
        /// </summary>
        public List<GoodsDataDto> GoodsData { get; set; }

        /// <summary>
        /// uid
        /// </summary>
        public string Uid { get; set; }

        /// <summary>
        /// 是否使用皮肤
        /// </summary>
        public bool UseSkin { get; set; }
    }

    public class GoodsDataDto
    {
        /// <summary>
        /// 物品全称
        /// </summary>
        public string GoodsName { get; set; }

        /// <summary>
        /// 排除改结果集后已拥有改物品的数量
        /// </summary>
        public int OwnedCount { get; set; }
    }



}
