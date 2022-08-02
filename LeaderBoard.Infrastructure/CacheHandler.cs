using System;
using System.Collections;
using System.Collections.Generic;

namespace LeaderBoard.Infrastructure
{
    /// <summary>
    /// to save score data
    /// </summary>
    public static class CacheHandler
    {             
        /// <summary>
        /// score rank list 
        /// </summary>
        public static List<Int64> CustomerRankList = new List<Int64>();

        /// <summary>
        /// list of customerId
        /// </summary>
        public static List<List<Int64>> CustomerIDList = new List<List<Int64>>();

        /// <summary>
        /// score list
        /// </summary>
        public static List<List<decimal>> ScoreList = new List<List<decimal>>();

        static CacheHandler()
        {
            // split customerId and score data into 11 parts
            for (int i = 0; i < 11; i++)
            {
                List<Int64> IDList = new List<Int64>();
                CustomerIDList.Add(IDList);

                List<decimal> scoreList = new List<decimal>();
                ScoreList.Add(scoreList);
            }                        
        }
    }
}
