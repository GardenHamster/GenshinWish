using GenshinWish.Type;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GenshinWish.Models.BO
{
    public class ProbabilityBO
    {
        /// <summary>
        /// 概率类型
        /// </summary>
        public ProbabilityType ProbabilityType { get; set; }

        /// <summary>
        /// 概率(百分比)
        /// </summary>
        public decimal Probability { get; set; }


        public ProbabilityBO(decimal probability, ProbabilityType probabilityType)
        {
            ProbabilityType = probabilityType;
            Probability = probability;
        }


    }
}
