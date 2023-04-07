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
        public int CountRate(int memberId, PoolType poolType, RareType rareType)
        {
            StringBuilder sqlBuilder = new StringBuilder();
            sqlBuilder.Append(" select count(mg.Id) count from member_goods mg");
            sqlBuilder.Append(" inner join goods g on g.Id=mg.GoodsId");
            sqlBuilder.Append(" where mg.MemberId=@MemberId and mg.AuthId=@AuthId and g.RareType=@RareType and mg.PoolType=@PoolType");
            return Db.Ado.SqlQuery<int>(sqlBuilder.ToString(), new { MemberId = memberId, PoolType = poolType, RareType = rareType }).First();
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
            sqlBuilder.Append(" select mg.GoodsId,g.GoodsName,g.GoodsType,g.GoodsSubType,g.RareType,mg.PoolType,mg.Cost,mg.CreateDate from member_goods mg");
            sqlBuilder.Append(" inner join goods g on g.Id=mg.GoodsId");
            sqlBuilder.Append(" where mg.MemberId=@MemberId and mg.AuthId=@AuthId and g.RareType=@RareType");
            sqlBuilder.Append(" order by mg.CreateDate desc limit @Top");
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
            sqlBuilder.Append(" select temp.AuthId, temp.MemberCode, m.MemberName, temp.RareType, temp.RareCount,");
            sqlBuilder.Append(" temp2.TotalPrayTimes, temp.rareCount/temp2.TotalPrayTimes as RareRate from member m");
            sqlBuilder.Append(" inner join (");
            sqlBuilder.Append(" 	select mg.AuthId,mg.MemberCode,g.RareType,count(g.RareType) RareCount from member_goods mg");
            sqlBuilder.Append("     inner join goods g on g.Id=mg.GoodsId");
            sqlBuilder.Append(" 	where mg.CreateDate>=@StartDate and mg.CreateDate<@EndDate and mg.AuthId=@AuthId and g.RareType=@RareType");
            sqlBuilder.Append(" 	group by mg.AuthId,mg.MemberCode,g.RareType limit @Top");
            sqlBuilder.Append(" ) temp on temp.MemberCode=m.MemberCode");
            sqlBuilder.Append(" inner join (");
            sqlBuilder.Append(" 	select AuthId,MemberCode,sum(PrayCount) TotalPrayTimes from pray_record");
            sqlBuilder.Append(" 	where CreateDate>=@StartDate and CreateDate<@EndDate and AuthId=@AuthId");
            sqlBuilder.Append(" 	group by AuthId,MemberCode");
            sqlBuilder.Append(" ) temp2 on temp2.MemberCode=m.MemberCode");
            sqlBuilder.Append(" where m.AuthId=@AuthId order by temp.RareType desc,rareRate desc,temp.RareCount asc");
            return Db.Ado.SqlQuery<LuckRankingDto>(sqlBuilder.ToString(), new { AuthId = authId, Top = top, RareType = rareType, StartDate = startDate, EndDate = endDate });
        }


    }
}
