using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GenshinWish.Exceptions
{
    public class BaseException : Exception
    {
        public int ErrorCode { get; set; }

        public BaseException(int errorCode, string message) : base(message)
        {
            ErrorCode = errorCode;
        }

    }
}
