using GenshinWish.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GenshinWish.Exceptions
{
    public class GoodsNotFoundException : BaseException
    {
        public GoodsNotFoundException(string message) : base(ResultCode.GoodsNotFound, message)
        {
        }

    }
}
