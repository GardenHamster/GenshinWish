using GenshinWish.Type;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GenshinWish.Models.PO
{
    [SugarTable("receive_record")]
    [SugarIndex("index_pr_MemberId", nameof(MemberId), OrderByType.Asc)]
    public class ReceiveRecordPO : BasePO
    {
        [SugarColumn(IsNullable = false, ColumnDescription = "成员ID")]
        public int MemberId { get; set; }

        [SugarColumn(IsNullable = false, ColumnDescription = "获得物品ID")]
        public int GoodsId { get; set; }

        [SugarColumn(IsNullable = false, ColumnDescription = "蛋池类型")]
        public WishType WishType { get; set; }

        [SugarColumn(IsNullable = false, ColumnDescription = "蛋池编号")]
        public int PoolIndex { get; set; }

        [SugarColumn(IsNullable = false, ColumnDescription = "累计消耗N抽")]
        public int Cost { get; set; }

        [SugarColumn(IsNullable = false, ColumnDescription = "添加日期")]
        public DateTime CreateDate { get; set; }

    }
}
