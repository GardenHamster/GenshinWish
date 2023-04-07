using GenshinWish.Models.BO;
using GenshinWish.Models.PO;
using GenshinWish.Type;
using System.Collections.Generic;
using System.Text;

namespace GenshinWish.Dao
{
    public class GoodsDao : DbContext<GoodsPO>
    {
        public List<GoodsPO> getGoodsList()
        {
            return Db.Queryable<GoodsPO>().OrderBy(o => o.Id).ToList();
        }

        public List<GoodsItemBO> getByGoodsType(GoodsType goodsType)
        {
            StringBuilder sqlBuilder = new StringBuilder();
            sqlBuilder.Append(" select g.Id as GoodsID,g.GoodsName,g.RareType,g.GoodsType,g.GoodsSubType from goods g");
            sqlBuilder.Append(" where g.GoodsType=@goodsType and g.isDisable=0");
            return Db.Ado.SqlQuery<GoodsItemBO>(sqlBuilder.ToString(), new { goodsType });
        }

        public GoodsPO getByGoodsName(string goodsName)
        {
            return Db.Queryable<GoodsPO>().Where(o => o.GoodsName == goodsName && o.IsDisable == false).First();
        }

        public List<GoodsItemBO> getPoolItems(int authId, PoolType poolType)
        {
            StringBuilder sqlBuilder = new StringBuilder();
            sqlBuilder.Append(" select g.GoodsName,g.RareType,g.GoodsType,g.GoodsSubType,pg.PoolIndex,pg.GoodsId from pool_goods pg");
            sqlBuilder.Append(" inner join goods g on g.id=pg.goodsId");
            sqlBuilder.Append(" where pg.AuthId=@authId and pg.PoolType=@PoolType and g.isDisable=0");
            return Db.Ado.SqlQuery<GoodsItemBO>(sqlBuilder.ToString(), new { authId, PoolType = poolType });
        }

        public List<GoodsItemBO> getPoolItems(int authId, PoolType poolType, int poolIndex)
        {
            StringBuilder sqlBuilder = new StringBuilder();
            sqlBuilder.Append(" select g.GoodsName,g.RareType,g.GoodsType,g.GoodsSubType,pg.PoolIndex,pg.GoodsId from pool_goods pg");
            sqlBuilder.Append(" inner join goods g on g.id=pg.goodsId");
            sqlBuilder.Append(" where pg.AuthId=@authId and pg.PoolType=@PoolType and pg.PoolIndex=@poolIndex and g.isDisable=0");
            return Db.Ado.SqlQuery<GoodsItemBO>(sqlBuilder.ToString(), new { authId, PoolType = poolType, poolIndex });
        }

        public List<GoodsPO> getStandardGoods(GoodsType goodsType, RareType rareType)
        {
            return Db.Queryable<GoodsPO>().Where(o => o.GoodsType == goodsType && o.RareType == rareType && o.IsPerm == true && o.IsDisable == false).ToList();
        }

    }
}
