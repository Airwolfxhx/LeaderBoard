using LeaderBoard.Domain;
using System;
using System.Collections.Generic;

namespace LeaderBoard.Application
{
    public interface ILeaderBoardService
    {
        List<CustomerScoreDto> GetCustomersByRank(int start, int end);

        /// <summary>
        /// get the found customer and its nearest neighborhoods
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="high">number of neighbors whose rank is higher than the specified customer</param>
        /// <param name="low">number of neighbors whose rank is lower than the specified customer</param>
        /// <returns></returns>
        APIResult<List<CustomerScoreDto>> GetCustomersByCustomerId(Int64 customerId, int high = 0, int low = 0);
    }
}
