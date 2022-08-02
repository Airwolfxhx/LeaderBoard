using LeaderBoard.Domain;
using LeaderBoard.Infrastructure;
using System;
using System.Collections.Generic;

namespace LeaderBoard.Application
{
    public class LeaderBoardService : ILeaderBoardService
    {
        /// <summary>
        /// get customer list by the range of rank 
        /// </summary>
        /// <param name="start">start rank</param>
        /// <param name="end">end rank</param>
        /// <returns></returns>
        public List<CustomerScore> GetCustomersByRank(int start, int end)
        {
            List<CustomerScore> list = new List<CustomerScore>();

            var dataCount = CacheHandler.CustomerRankList.Count;
            if (end > dataCount)
                end = dataCount;

            for (int i = start - 1; i < end; i++)
            {
                CustomerScore obj = GetCustomerScoreByIndex(i);

                list.Add(obj);
            }

            return list;
        }

        CustomerScore GetCustomerScoreByIndex(int rankIndex)
        {
            CustomerScore obj = new CustomerScore();
            obj.CustomerId = CacheHandler.CustomerRankList[rankIndex];
            obj.Rank = rankIndex + 1;

            for (int j = 0; j < 10; j++)
            {
                int index = CacheHandler.CustomerIDList[j].FindIndex(x => x == obj.CustomerId);
                if (index != -1)
                {
                    obj.Score = CacheHandler.ScoreList[j][index];
                    break;
                }
            }

            return obj;
        }

        /// <summary>
        /// get the found customer and its nearest neighborhoods
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="high">number of neighbors whose rank is higher than the specified customer</param>
        /// <param name="low">number of neighbors whose rank is lower than the specified customer</param>
        /// <returns></returns>
        public APIResult<List<CustomerScore>> GetCustomersByCustomerId(Int64 customerId, int high = 0, int low = 0)
        {
            APIResult<List<CustomerScore>> result = new APIResult<List<CustomerScore>>();

            List<CustomerScore> list = new List<CustomerScore>();

            int index = CacheHandler.CustomerRankList.FindIndex(x => x == customerId);

            if (index == -1)
            {
                result.ErrorCode = Domain.Enum.EnumError.NoData;
                result.ErrorMessage = "Cannot find the customer";

                return result;
            }

            // rank list count
            int count = CacheHandler.CustomerRankList.Count;

            // get customer by customerId
            CustomerScore obj = GetCustomerScoreByIndex(index);
            list.Add(obj);

            // get high neighbors
            if (high != 0)
            {
                if (index - high <= 0)
                {
                    high = 0;
                }
                else
                    high = index - high;

                for (int i = index-1; i >=high; i--)
                {
                    obj = GetCustomerScoreByIndex(i);
                    list.Insert(0,obj);
                }
            }

            // get low neighbors
            if (low != 0)
            {
                if (index + low >= count)
                {
                    low = count-1;
                }
                else
                    low = index + low;

                for (int i = index+1; i <= low; i++)
                {
                    obj = GetCustomerScoreByIndex(i);
                    list.Add(obj);
                }
            }

            result.Data = list;
            return result;
        }
    }
}
