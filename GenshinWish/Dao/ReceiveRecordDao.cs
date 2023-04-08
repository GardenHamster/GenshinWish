using System.Collections.Generic;
using System.Text;
using System;
using System.Linq;
using GenshinWish.Models.PO;
using GenshinWish.Models.DTO;
using GenshinWish.Type;

namespace GenshinWish.Dao
{
    public class ReceiveRecordDao : DbContext<ReceiveRecordPO>
    {
        /// <summary>
        /// 统计某个成员某个蛋池类型某个星级物品的出货数量
        /// </summary>
        /// <param name="memberId"></param>
        /// <param name="poolType"></param>
        /// <param name="rareType"></param>
        /// <returns></returns>
        public decimal CountRate(int memberId, PoolType poolType, RareType rareType)
        {
            StringBuilder sqlBuilder = new StringBuilder();
            sqlBuilder.Append(" select IFNULL(count(r.Id)/sum(r.Cost),0) rate from receive_record r");
            sqlBuilder.Append(" inner join goods g on g.Id=r.GoodsId");
            sqlBuilder.Append(" where r.MemberId=@MemberId and r.PoolType=@PoolType and g.RareType=@RareType");
            return Db.Ado.SqlQuery<decimal>(sqlBuilder.ToString(), new { MemberId = memberId, PoolType = poolType, RareType = rareType }).First();
        }

        /// <summary>
        /// 读取出货记录
        /// </summary>
        /// <param name="memberId"></param>
        /// <param name="rareType"></param>
        /// <param name="top"></param>
        /// <returns></returns>
        public List<ReceiveRecordDto> GetRecords(int memberId, RareType rareType, int top)
        {
            StringBuilder sqlBuilder = new StringBuilder();
            sqlBuilder.Append(" select r.GoodsId,g.GoodsName,g.GoodsType,g.GoodsSubType,g.RareType,r.PoolType,r.Cost,r.CreateDate from receive_record r");
            sqlBuilder.Append(" inner join goods g on g.Id=r.GoodsId");
            sqlBuilder.Append(" where r.MemberId=@MemberId and g.RareType=@RareType");
            sqlBuilder.Append(" order by r.Id desc limit @Top");
            return Db.Ado.SqlQuery<ReceiveRecordDto>(sqlBuilder.ToString(), new { MemberId = memberId, RareType = rareType, Top = top });
        }

        /// <summary>
        /// 统计一个时间段内，同一个authId下某一个星级的出货率排行
        /// </summary>
        /// <param name="authId"></param>
        /// <param name="rareType"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="top"></param>
        /// <returns></returns>
        public List<LuckRankingDto> GetLuckRanking(int authId, RareType rareType, DateTime startDate, DateTime endDate, int top)
        {
            StringBuilder sqlBuilder = new StringBuilder();
            sqlBuilder.Append(" select temp.*,(temp.ReceiveCount/temp.WishTimes) ReceiveRate from(");
            sqlBuilder.Append("   select m.AuthId,m.MemberCode,m.MemberName,g.RareType,count(r.MemberId) ReceiveCount,sum(r.Cost) WishTimes from receive_record r");
            sqlBuilder.Append("   inner join member m on m.Id=r.MemberId");
            sqlBuilder.Append("   inner join goods g on g.Id=r.GoodsId");
            sqlBuilder.Append("   where r.CreateDate>@StartDate and r.CreateDate<@EndDate and m.AuthId=@AuthId and g.RareType=@RareType");
            sqlBuilder.Append("   group by r.MemberId");
            sqlBuilder.Append(" ) temp order by ReceiveRate desc limit @Top");
            return Db.Ado.SqlQuery<LuckRankingDto>(sqlBuilder.ToString(), new { AuthId = authId, Top = top, RareType = rareType, StartDate = startDate, EndDate = endDate });
        }


    }
}
