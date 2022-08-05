using LeaderBoard.Domain;
using LeaderBoard.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace LeaderBoard.Application
{
    public class CommonService : ICommonService
    {
        /// <summary>
        /// binary search the index of rank list to add or remove
        /// </summary>
        /// <param name="customer">the customer to add</param>
        /// <returns></returns>
        public int BinarySearch(CustomerScore customer)
        {
            ScoreComparer scoreComparer = new ScoreComparer();
            return CacheHandler.CustomerRankList.BinarySearch(customer, scoreComparer);
        }
    }
}
