using LeaderBoard.Domain;
using LeaderBoard.Infrastructure;
using System;
using System.Collections.Generic;
using System.Threading;

namespace LeaderBoard.Application
{
    public class LeaderBoardService : ILeaderBoardService
    {
        private readonly ICommonService _commonService;
        private ReaderWriterLock _lock;

        public LeaderBoardService(ICommonService commonService)
        {
            _commonService = commonService;
        }

        /// <summary>
        /// get customer list by the range of rank 
        /// </summary>
        /// <param name="start">start rank</param>
        /// <param name="end">end rank</param>
        /// <returns></returns>
        public List<CustomerScoreDto> GetCustomersByRank(int start, int end)
        {
            List<CustomerScoreDto> list = new List<CustomerScoreDto>();

            CacheHandler.CustomerLock.AcquireReaderLock(100);
            try
            { 
                var dataCount = CacheHandler.CustomerRankList.Count;
                if (end > dataCount)
                    end = dataCount;

                for (int i = start - 1; i < end; i++)
                {
                    CustomerScoreDto obj = GetCustomerScoreByIndex(i);
                    obj.Rank = i + 1;

                    list.Add(obj);
                }
            }            
            finally
            {
                CacheHandler.CustomerLock.ReleaseReaderLock();
            }
            return list;
        }

        CustomerScoreDto GetCustomerScoreByIndex(int index)
        {
            CustomerScoreDto obj = new CustomerScoreDto();
            obj.CustomerId = CacheHandler.CustomerRankList[index].CustomerId;
            obj.Score = CacheHandler.CustomerRankList[index].Score;

            return obj;
        }

        /// <summary>
        /// get the found customer and its nearest neighborhoods
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="high">number of neighbors whose rank is higher than the specified customer</param>
        /// <param name="low">number of neighbors whose rank is lower than the specified customer</param>
        /// <returns></returns>
        public APIResult<List<CustomerScoreDto>> GetCustomersByCustomerId(Int64 customerId, int high = 0, int low = 0)
        {
            APIResult<List<CustomerScoreDto>> result = new APIResult<List<CustomerScoreDto>>();

            List<CustomerScoreDto> list = new List<CustomerScoreDto>();

            CacheHandler.CustomerLock.AcquireReaderLock(100);
            try
            {               
                if (!CacheHandler.CustomerMap.ContainsKey(customerId))
                {
                    result.ErrorCode = Domain.Enum.EnumError.NoData;
                    result.ErrorMessage = "Cannot find the customer.";

                    return result;
                }

                var customer = CacheHandler.CustomerMap[customerId];

                // rank list count
                int count = CacheHandler.CustomerRankList.Count;

                // get index in rank list by customerId
                int index = _commonService.BinarySearch(customer);
                
                CustomerScoreDto customerDto = new CustomerScoreDto()
                {
                    CustomerId = customer.CustomerId,
                    Score = customer.Score,
                    Rank = index + 1
                };
                list.Add(customerDto);

                // get high neighbors
                if (high != 0)
                {
                    if (index - high <= 0)
                    {
                        high = 0;
                    }
                    else
                        high = index - high;

                    for (int i = index - 1; i >= high; i--)
                    {
                        CustomerScoreDto obj = GetCustomerScoreByIndex(i);
                        obj.Rank = i + 1;
                        list.Insert(0, obj);
                    }
                }

                // get low neighbors
                if (low != 0)
                {
                    if (index + low >= count)
                    {
                        low = count - 1;
                    }
                    else
                        low = index + low;

                    for (int i = index + 1; i <= low; i++)
                    {
                        CustomerScoreDto obj = GetCustomerScoreByIndex(i);
                        obj.Rank = i + 1;
                        list.Add(obj);
                    }
                }
            }
            finally
            {
                CacheHandler.CustomerLock.ReleaseReaderLock();
            }

            result.Data = list;
            return result;
        }
    }
}
