using LeaderBoard.Domain;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

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
        public static List<CustomerScore> CustomerRankList = new List<CustomerScore>();

        /// <summary>
        /// customer data
        /// </summary>
        public static Dictionary<long, CustomerScore> CustomerMap = new Dictionary<long, CustomerScore>();

        /// <summary>
        /// the maximum score in rank list
        /// </summary>
        public static decimal MaxScore = 0;

        /// <summary>
        /// the minimum score in rank list
        /// </summary>
        public static decimal MinScore = 0;

        public static ReaderWriterLock CustomerLock = new ReaderWriterLock();          
    }
}
