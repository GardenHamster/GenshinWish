using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GenshinWish.Models.PO
{
    [SugarTable("member")]
    public class MemberPO : BasePO
    {
        [SugarColumn(IsNullable = false, ColumnDescription = "授权码ID")]
        public int AuthId { get; set; }

        [SugarColumn(IsNullable = false, Length = 32, ColumnDescription = "成员编号")]
        public string MemberCode { get; set; }

        [SugarColumn(IsNullable = true, Length = 20, DefaultValue = "", ColumnDescription = "成员名称")]
        public string MemberName { get; set; }

        [SugarColumn(IsNullable = false, DefaultValue = "180", ColumnDescription = "角色池剩余多少发五星大保底")]
        public int Char180Surplus { get; set; }

        [SugarColumn(IsNullable = false, DefaultValue = "20", ColumnDescription = "角色池剩余多少发十连大保底")]
        public int Char20Surplus { get; set; }

        [SugarColumn(IsNullable = false, DefaultValue = "0", ColumnDescription = "武器定轨Id，0表示无定轨")]
        public int AssignId { get; set; }

        [SugarColumn(IsNullable = false, DefaultValue = "0", ColumnDescription = "武器池命定值")]
        public int AssignValue { get; set; }

        [SugarColumn(IsNullable = false, DefaultValue = "80", ColumnDescription = "武器池剩余多少发五星保底")]
        public int Wpn80Surplus { get; set; }

        [SugarColumn(IsNullable = false, DefaultValue = "20", ColumnDescription = "武器池剩余多少发十连大保底")]
        public int Wpn20Surplus { get; set; }

        [SugarColumn(IsNullable = false, DefaultValue = "90", ColumnDescription = "常驻池剩余多少发五星保底")]
        public int Std90Surplus { get; set; }

        [SugarColumn(IsNullable = false, DefaultValue = "10", ColumnDescription = "常驻池剩余多少发十连保底")]
        public int Std10Surplus { get; set; }

        [SugarColumn(IsNullable = false, DefaultValue = "90", ColumnDescription = "全角色池剩余多少发五星保底")]
        public int FullChar90Surplus { get; set; }

        [SugarColumn(IsNullable = false, DefaultValue = "10", ColumnDescription = "全角色池剩余多少发十连保底")]
        public int FullChar10Surplus { get; set; }

        [SugarColumn(IsNullable = false, DefaultValue = "80", ColumnDescription = "全武器池剩余多少发五星保底")]
        public int FullWpn80Surplus { get; set; }

        [SugarColumn(IsNullable = false, DefaultValue = "10", ColumnDescription = "全武器池剩余多少发十连保底")]
        public int FullWpn10Surplus { get; set; }

        public MemberPO() { }

        public MemberPO(int authId, string memberCode, string memberName)
        {
            AuthId = authId;
            MemberCode = memberCode;
            MemberName = memberName;
            Char180Surplus = 180;
            Char20Surplus = 20;
            Wpn80Surplus = 80;
            Wpn20Surplus = 20;
            Std90Surplus = 90;
            Std10Surplus = 10;
            FullChar90Surplus = 90;
            FullChar10Surplus = 10;
            FullWpn80Surplus = 80;
            FullWpn10Surplus = 10;
        }

        public void ResetSurplus()
        {
            Char180Surplus = 180;
            Char20Surplus = 20;
            Wpn80Surplus = 80;
            Wpn20Surplus = 20;
            Std90Surplus = 90;
            Std10Surplus = 10;
            FullChar90Surplus = 90;
            FullChar10Surplus = 10;
            FullWpn80Surplus = 80;
            FullWpn10Surplus = 10;
            AssignId = 0;
            AssignValue = 0;
        }

    }
}
