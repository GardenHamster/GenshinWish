using GenshinWish.Type;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GenshinWish.Models.PO
{
    [SugarTable("wish_record")]
    [SugarIndex("index_wr_MemberId", nameof(MemberId), OrderByType.Asc)]
    public class WishRecordPO : BasePO
    {
        [SugarColumn(IsNullable = false, ColumnDescription = "成员Id")]
        public int MemberId { get; set; }

        [SugarColumn(IsNullable = false, ColumnDescription = "蛋池类型")]
        public PoolType PoolType { get; set; }

        [SugarColumn(IsNullable = false, ColumnDescription = "蛋池编号")]
        public int WishIndex { get; set; }

        [SugarColumn(IsNullable = false, ColumnDescription = "祈愿次数")]
        public int WishCount { get; set; }

        [SugarColumn(IsNullable = false, ColumnDescription = "祈愿日期")]
        public DateTime CreateDate { get; set; }
    }
}
