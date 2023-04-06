using GenshinWish.Models.PO;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace GenshinWish.Models.BO
{
    public class WishResultBO
    {
        public AuthorizePO Authorize { get; set; }

        public MemberPO MemberInfo { get; set; }

        public WishRecordBO[] WishRecords { get; set; }

        public WishRecordBO[] SortWishRecords { get; set; }

        public int Star5Cost { get; set; }

        public int Surplus10 { get; set; }
    }
}
