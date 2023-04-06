﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GenshinWish.Common
{
    public class ResultCode
    {
        /// <summary>
        /// 成功
        /// </summary>
        public static readonly int Success = 0;

        /// <summary>
        /// 错误
        /// </summary>
        public static readonly int Error = 600;

        /// <summary>
        /// 超过api每日调用上限
        /// </summary>
        public static readonly int ApiMaximum = 601;

        /// <summary>
        /// 参数无效
        /// </summary>
        public static readonly int InvalidParameter = 602;

        /// <summary>
        /// 找不到物品
        /// </summary>
        public static readonly int GoodsNotFound = 603;

        /// <summary>
        /// Api限制
        /// </summary>
        public static readonly int ApiLimit = 604;

        /// <summary>
        /// 当前up池中不存在待定轨的物品
        /// </summary>
        public static readonly int AssignNotFound = 605;

        /// <summary>
        /// 卡池未配置
        /// </summary>
        public static readonly int PoolNotConfigured = 606;

        /// <summary>
        /// 权限不足
        /// </summary>
        public static readonly int PermissionDenied = 607;



    }
}
