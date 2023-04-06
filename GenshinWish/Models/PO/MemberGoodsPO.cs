using SqlSugar;

namespace GenshinWish.Models.PO
{
    [SugarTable("member_goods")]
    [SugarIndex("index_mg_MemberId", nameof(MemberId), OrderByType.Asc)]
    public class MemberGoodsPO : BasePO
    {
        [SugarColumn(IsNullable = false, ColumnDescription = "成员ID")]
        public int MemberId { get; set; }

        [SugarColumn(IsNullable = false, ColumnDescription = "物品ID")]
        public int GoodsId { get; set; }

        [SugarColumn(IsNullable = false, ColumnDescription = "拥有数量")]
        public int Count { get; set; }

        public MemberGoodsPO() { }

        public MemberGoodsPO(int memberId, int goodsId, int count)
        {
            MemberId = memberId;
            GoodsId = goodsId;
            Count = count;
        }

    }
}
