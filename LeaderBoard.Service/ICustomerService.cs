using System;

namespace LeaderBoard.Application
{
    public interface ICustomerService
    {
        /// <summary>
        /// save score of customer
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="score"></param>
        bool SaveScore(Int64 customerId, decimal score);
    }
}
