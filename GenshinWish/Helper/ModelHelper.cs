using GenshinWish.Models.BO;
using GenshinWish.Models.DTO;
using GenshinWish.Models.PO;
using GenshinWish.Models.VO;
using System.Collections.Generic;
using System.Linq;

namespace GenshinWish.Helper
{
    public static class ModelHelper
    {
        /// <summary>
        /// 转换为WishRecordVO
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static List<WishRecordVO> ToWishRecordVO(this List<ReceiveRecordDto> list)
        {
            return list.Select(o => new WishRecordVO(o)).ToList();
        }

        /// <summary>
        /// 转换为GoodsVO
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static List<GoodsVO> ToGoodsVO(this WishRecordBO[] array)
        {
            return array.Select(o => new GoodsVO(o)).ToList();
        }

        /// <summary>
        /// 转换为GoodsVO
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static List<GoodsVO> ToGoodsVO(this List<GoodsItemBO> list)
        {
            return list.Select(o => new GoodsVO(o)).ToList();
        }

        /// <summary>
        /// 转化为GoodsItem
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static List<GoodsItemBO> ToGoodsItem(this List<GoodsPO> list)
        {
            return list.Select(o => new GoodsItemBO(o)).ToList();
        }

    }
}
