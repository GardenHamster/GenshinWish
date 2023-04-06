using GenshinWish.Models.PO;
using System.Collections.Generic;

namespace GenshinWish.Models.DTO
{
    public class InitDataDto
    {
        public List<GoodsPO> Goods { get; set; }

        public List<GoodsPoolPO> GoodsPools { get; set; }
    }
}
