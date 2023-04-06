using GenshinWish.Models.BO;
using System.Collections.Generic;

namespace GenshinWish.Cache
{
    public static class DefaultPool
    {
        /// <summary>
        /// 5星角色常驻列表
        /// </summary>
        public static List<GoodsItemBO> Star5CharacterItems = new List<GoodsItemBO>();

        /// <summary>
        /// 4星角色常驻列表
        /// </summary>
        public static List<GoodsItemBO> Star4CharacterItems = new List<GoodsItemBO>();

        /// <summary>
        /// 5星武器常驻列表
        /// </summary>
        public static List<GoodsItemBO> Star5WeaponItems = new List<GoodsItemBO>();

        /// <summary>
        /// 4星武器常驻列表
        /// </summary>
        public static List<GoodsItemBO> Star4WeaponItems = new List<GoodsItemBO>();

        /// <summary>
        /// 5星常驻列表
        /// </summary>
        public static List<GoodsItemBO> Star5FullItems = new List<GoodsItemBO>();

        /// <summary>
        /// 4星常驻列表
        /// </summary>
        public static List<GoodsItemBO> Star4FullItems = new List<GoodsItemBO>();

        /// <summary>
        /// 3星常驻列表
        /// </summary>
        public static List<GoodsItemBO> Star3FullItems = new List<GoodsItemBO>();

        /// <summary>
        /// 默认武器池
        /// </summary>
        public static Dictionary<int, UpItemBO> WeaponPools = new Dictionary<int, UpItemBO>();

        /// <summary>
        /// 默认角色池
        /// </summary>
        public static Dictionary<int, UpItemBO> CharacterPools = new Dictionary<int, UpItemBO>();

        /// <summary>
        /// 默认常驻池
        /// </summary>
        public static UpItemBO StandardPool = new UpItemBO();

        /// <summary>
        /// 全武器池
        /// </summary>
        public static UpItemBO FullWeaponPool = new UpItemBO();

        /// <summary>
        /// 全角色池
        /// </summary>
        public static UpItemBO FullCharacterPool = new UpItemBO();
    }
}
