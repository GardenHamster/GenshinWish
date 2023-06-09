﻿using GenshinWish.Models.PO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GenshinWish.Dao
{
    public class AuthorizeDao : DbContext<AuthorizePO>
    {
        public AuthorizePO GetAuthorize(string code)
        {
            return Db.Queryable<AuthorizePO>().Where(o => o.Code == code).First();
        }

    }
}
