using LeaderBoard.Domain;
using LeaderBoard.Infrastructure;
using System;
using System.Collections.Generic;

namespace LeaderBoard.Application
{
    public class CustomerService : ICustomerService
    {
        private readonly ICommonService _commonService;

        public CustomerService(ICommonService commonService)
        {
            _commonService = commonService;
        }

        /// <summary>
        /// save score of customer
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="score"></param>
        public bool SaveScore(Int64 customerId, decimal score)
        {            
            try
            {
                CacheHandler.CustomerLock.AcquireWriterLock(100);
                try
                {                    
                    if (!CacheHandler.CustomerMap.ContainsKey(customerId))
                    {
                        // opps~
                        if (CacheHandler.CustomerRankList.Count == int.MaxValue)
                        {
                            return false;
                        }

                        AddCustomer(customerId, score);
                    }
                    else
                    {
                        UpdateCustomer(customerId, score);
                    }
                }
                finally
                {
                    CacheHandler.CustomerLock.ReleaseWriterLock();
                }
                return true;
            }
            catch
            {
                throw;
            }            
        }

        #region Add Customer
        /// <summary>
        /// add customer to list
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="score"></param>
        void AddCustomer(Int64 customerId, decimal score)
        {
            CustomerScore customer = new CustomerScore()
            {
                CustomerId = customerId,
                Score = score
            };

            CacheHandler.CustomerMap.Add(customerId, customer);

            // no need to save rank
            if (score <= 0)
            {
                return;
            }

            int count = CacheHandler.CustomerRankList.Count;

            if (count == 0)
            {
                CacheHandler.CustomerRankList.Add(customer);
                return;
            }

            // greater than the max value
            if (score >= CacheHandler.MaxScore)
            {
                CacheHandler.MaxScore = score;
                AddScoreWithCurrentIndex(0, customer);
                return;
            }

            // lower than the min value
            if (score <= CacheHandler.MinScore)
            {
                CacheHandler.MinScore = score;
                AddScoreWithCurrentIndex(count - 1, customer);
                return;
            }

            int index = _commonService.BinarySearch(customer);

            if (index < 0)
            {
                CacheHandler.CustomerRankList.Insert(~index, customer);
            }
        }

        /// <summary>
        /// add score with the same value
        /// </summary>
        /// <param name="area">the data area</param>
        /// <param name="currentIndex">the index to insert data</param>
        /// <param name="customerId"></param>
        /// <param name="score"></param>
        void AddScoreWithCurrentIndex(int currentIndex, CustomerScore objectCustomer)
        {
            CustomerScore currentCustomer = CacheHandler.CustomerRankList[currentIndex];

            if (objectCustomer.Score == currentCustomer.Score)
            {
                if (currentCustomer.CustomerId < objectCustomer.CustomerId)
                {
                    currentIndex++;
                }
            }

            if (currentCustomer.Score > objectCustomer.Score)
            {
                currentIndex++;
            }

            CacheHandler.CustomerRankList.Insert(currentIndex, objectCustomer);
        }
        #endregion        

        /// <summary>
        /// update customer data
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="score"></param>
        /// <param name="oldScore"></param>
        void UpdateCustomer(Int64 customerId, decimal score)
        {            
            decimal oldScore = CacheHandler.CustomerMap[customerId].Score;

            CacheHandler.CustomerMap.Remove(customerId);

            if (oldScore > 0)
            {
                CustomerScore customer = new CustomerScore()
                {
                    CustomerId = customerId,
                    Score = oldScore
                };
                
                int index = _commonService.BinarySearch(customer);

                if(index>=0)
                    CacheHandler.CustomerRankList.RemoveAt(index);
            }

            AddCustomer(customerId, oldScore + score);
        }
    }
}
