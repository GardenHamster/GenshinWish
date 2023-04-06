﻿using GenshinWish.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GenshinWish.Exceptions
{
    public class ParamException : BaseException
    {
        public ParamException(string message) : base(ResultCode.InvalidParameter, message)
        {

        }
    }
}
