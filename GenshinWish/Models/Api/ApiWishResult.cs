using GenshinWish.Models.VO;
using System.Collections.Generic;

namespace GenshinWish.Models.Api
{
    public class ApiWishResult
    {
        /// <summary>
        /// 祈愿次数
        /// </summary>
        public int WishCount { get; set; }

        /// <summary>
        /// 角色池剩余多少抽五星大保底
        /// </summary>
        public int Character180Surplus { get; set; }

        /// <summary>
        /// 角色池剩余多少抽五星小保底
        /// </summary>
        public int Character90Surplus { get; set; }

        /// <summary>
        /// 武器池剩余多少抽五星保底
        /// </summary>
        public int Weapon80Surplus { get; set; }

        /// <summary>
        /// 武器池命定值
        /// </summary>
        public int WeaponAssignValue { get; set; }

        /// <summary>
        /// 常驻池剩余多少抽五星保底
        /// </summary>
        public int Standard90Surplus { get; set; }

        /// <summary>
        /// 全角色池剩余多少抽五星小保底
        /// </summary>
        public int FullCharacter90Surplus { get; set; }

        /// <summary>
        /// 全武器池剩余多少抽五星保底
        /// </summary>
        public int FullWeapon80Surplus { get; set; }

        /// <summary>
        /// api当天剩余调用次数
        /// </summary>
        public int ApiDailyCallSurplus { get; set; }

        /// <summary>
        /// 图片路径
        /// </summary>
        public string ImgHttpUrl { get; set; }

        /// <summary>
        /// 图片路径
        /// </summary>
        public long ImgSize { get; set; }

        /// <summary>
        /// 图片相对路径
        /// </summary>
        public string ImgPath { get; set; }

        /// <summary>
        /// base64
        /// </summary>
        public string ImgBase64 { get; set; }

        /// <summary>
        /// 获得的3星物品列表
        /// </summary>
        public List<GoodsVO> Star3Goods { get; set; }

        /// <summary>
        /// 获得的4星物品列表
        /// </summary>
        public List<GoodsVO> Star4Goods { get; set; }

        /// <summary>
        /// 获得的5星物品列表
        /// </summary>
        public List<GoodsVO> Star5Goods { get; set; }

        /// <summary>
        /// 当前4星up物品
        /// </summary>
        public List<GoodsVO> Star4Up { get; set; }

        /// <summary>
        /// 当前54星up物品
        /// </summary>
        public List<GoodsVO> Star5Up { get; set; }

    }
}
