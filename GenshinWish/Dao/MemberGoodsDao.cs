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
        public List<MemberGoodsDto> GetGoods(int memberId)
        {
            StringBuilder sqlBuilder = new StringBuilder();
            sqlBuilder.Append(" select g.Id,g.GoodsName,g.GoodsType,g.RareType,mg.Count from member_goods mg");
            sqlBuilder.Append(" inner join goods g on g.Id=mg.GoodsId");
            sqlBuilder.Append(" where mg.MemberId=@MemberId");
            return Db.Ado.SqlQuery<MemberGoodsDto>(sqlBuilder.ToString(), new { MemberId = memberId });
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
