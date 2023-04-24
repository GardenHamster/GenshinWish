using GenshinWish.Models.DTO;
using GenshinWish.Models.PO;
using GenshinWish.Type;
using System.Collections.Generic;
using System.Text;

namespace GenshinWish.Dao
{
    public class MemberGoodsDao : DbContext<MemberGoodsPO>
    {
        /// <summary>
        /// 获取成员已获得的物品
        /// </summary>
        /// <param name="memberId"></param>
        /// <returns></returns>
        public List<MemberGoodsBO> GetGoods(int memberId)
        {
            return Db.Queryable<MemberGoodsPO>()
            .InnerJoin<GoodsPO>((mg, g) => mg.GoodsId == g.Id)
            .Where((mg, g) => mg.MemberId == memberId)
            .Select((mg, g) => new MemberGoodsBO
            {
                GoodsId = g.Id,
                GoodsName = g.GoodsName,
                GoodsType = g.GoodsType,
                RareType = g.RareType,
                Count = mg.Count
            }).ToList();
        }

        /// <summary>
        /// 统计物品数量
        /// </summary>
        /// <param name="memberId"></param>
        /// <param name="rareType"></param>
        /// <returns></returns>
        public int CountGoods(int memberId, RareType rareType)
        {
            return Db.Queryable<MemberGoodsPO>()
            .InnerJoin<GoodsPO>((mg, g) => mg.GoodsId == g.Id)
            .Where((mg, g) => mg.MemberId == memberId && g.RareType == rareType)
            .Sum(mg => mg.Count);
        }

        /// <summary>
        /// 物品重置
        /// </summary>
        /// <param name="memberId"></param>
        /// <returns></returns>
        public int ResetGoods(int memberId)
        {
            return Db.Deleteable<MemberGoodsPO>().Where(o => o.MemberId == memberId).ExecuteCommand();
        }

        /// <summary>
        /// 修改物品数量
        /// </summary>
        /// <param name="memberId"></param>
        /// <param name="goodsId"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public int AddCount(int memberId, int goodsId, int count)
        {
            return Db.Updateable<MemberGoodsPO>()
            .SetColumns(o => o.Count == o.Count + count)
            .Where(o => o.MemberId == memberId && o.GoodsId == goodsId)
            .ExecuteCommand();
        }

    }
}
