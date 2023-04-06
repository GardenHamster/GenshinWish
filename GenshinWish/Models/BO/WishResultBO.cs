using GenshinWish.Models.PO;
using GenshinWish.Type;
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

        public int PoolIndex { get; set; }

    }
}
