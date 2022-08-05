using LeaderBoard.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace LeaderBoard.Application
{
    public interface ICommonService
    {
        /// <summary>
        /// binary search the index of rank list to add or remove
        /// </summary>
        /// <param name="customer">the customer to add</param>
        /// <returns></returns>
        int BinarySearch(CustomerScore customer);
    }
}
