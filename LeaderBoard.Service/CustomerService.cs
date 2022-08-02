using LeaderBoard.Domain;
using LeaderBoard.Infrastructure;
using System;
using System.Collections.Generic;

namespace LeaderBoard.Application
{
    public class CustomerService : ICustomerService
    {
        private object thisObject = new object();

        /// <summary>
        /// save score of customer
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="score"></param>
        public bool SaveScore(Int64 customerId, decimal score)
        {
            lock (thisObject)
            {
                try
                {
                    if (!Exists(customerId))
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

                    return true;
                }
                catch
                {
                    throw;
                }

            }
        }

        #region Add Customer
        /// <summary>
        /// does ID exist
        /// </summary>
        /// <param name="customerId"></param>
        /// <returns></returns>
        bool Exists(Int64 customerId)
        {
            return CacheHandler.CustomerRankList.Find(x => x == customerId) > 0;
        }

        /// <summary>
        /// add customer to list
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="score"></param>
        void AddCustomer(Int64 customerId, decimal score)
        {
            // save to negative list
            if (score <= 0)
            {
                CacheHandler.CustomerIDList[10].Add(customerId);
                CacheHandler.ScoreList[10].Add(score);
            }

            // get area by score
            int area = (int)score / 100;

            if (score == 1000)
                area = 9;

            // sign the customerID to insert
            Int64 signCustomerID = 0;
            // the count of customer list
            int count = CacheHandler.ScoreList[area].Count;

            if (CacheHandler.ScoreList[area].Count == 0)
            {
                // insert data to area
                CacheHandler.ScoreList[area].Add(score);
                CacheHandler.CustomerIDList[area].Add(customerId);

                if (area == 0)
                {
                    // add customer ID to bottom of index list
                    CacheHandler.CustomerRankList.Add(customerId);
                }
                else
                {
                    // seach the area before current
                    bool lowerDataExists = false;
                    for (int n = area - 1; n >= 0; n--)
                    {
                        if (CacheHandler.CustomerIDList[n].Count == 0)
                            continue;
                        else
                        {
                            lowerDataExists = true;

                            signCustomerID = CacheHandler.CustomerIDList[n][0];
                            int index = CacheHandler.CustomerRankList.FindIndex(x => x == signCustomerID);
                            CacheHandler.CustomerRankList.Insert(index, customerId);
                            break;
                        }
                    }

                    // if there is no data
                    if (!lowerDataExists)
                        CacheHandler.CustomerRankList.Add(customerId);
                }

                return;
            }

            // greater than the max value
            if (CacheHandler.ScoreList[area][0] <= score)
            {
                AddScoreWithCurrentIndex(area, 0, customerId, score);
                return;
            }

            // lower than the min value
            if (CacheHandler.ScoreList[area][count - 1] >= score)
            {
                AddScoreWithCurrentIndex(area, count - 1, customerId, score);
                return;
            }

            // the last index of value which is greater than score
            int lastMax = 0;

            int maxIndex = FindMaxIndex(CacheHandler.ScoreList[area], 0, count - 1, score, lastMax);

            AddScoreWithCurrentIndex(area, maxIndex, customerId, score);
        }

        /// <summary>
        /// add score with the same value
        /// </summary>
        /// <param name="area">the data area</param>
        /// <param name="currentIndex">the index to insert data</param>
        /// <param name="customerId"></param>
        /// <param name="score"></param>
        void AddScoreWithCurrentIndex(int area, int currentIndex, Int64 customerId, decimal score)
        {
            decimal currentScore = CacheHandler.ScoreList[area][currentIndex];

            Int64 currentCustomerID = CacheHandler.CustomerIDList[area][currentIndex];

            int rankIndex = CacheHandler.CustomerRankList.FindIndex(x => x == currentCustomerID);

            if (currentScore == score)
            {
                // find the first index of max score
                currentIndex = CacheHandler.ScoreList[area].FindIndex(x => x == currentScore);

                currentCustomerID = CacheHandler.CustomerIDList[area][currentIndex];

                rankIndex = CacheHandler.CustomerRankList.FindIndex(x => x == currentCustomerID);

                if (currentCustomerID < customerId)
                {
                    currentIndex++;
                    rankIndex++;
                }
            }

            if (currentScore > score)
            {
                currentIndex++;
                rankIndex++;
            }

            // insert data to area
            CacheHandler.ScoreList[area].Insert(currentIndex, score);
            CacheHandler.CustomerIDList[area].Insert(currentIndex, customerId);

            // insert data to rank list
            CacheHandler.CustomerRankList.Insert(rankIndex, customerId);
        }
        #endregion

        /// <summary>
        /// find the index of maximum approximation
        /// </summary>
        /// <param name="scoreList"></param>
        /// <param name="low"></param>
        /// <param name="high"></param>
        /// <param name="score"></param>
        /// <param name="lastMax">the last index of score which is greater than current score</param>
        /// <returns></returns>
        public int FindMaxIndex(List<decimal> scoreList, int low, int high, decimal score,int lastMax)
        {            
            int mid = (low + high) / 2;
            if (low > high)
                return lastMax;
            else
            {
                if (scoreList[mid] == score)
                    return mid;
                else if (scoreList[mid] > score)
                {
                    lastMax = mid;
                    return FindMaxIndex(scoreList, low, mid - 1, score, lastMax);
                }
                else
                    return FindMaxIndex(scoreList, mid + 1, high, score, lastMax);
            }
        }

        /// <summary>
        /// update customer data
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="score"></param>
        void UpdateCustomer(Int64 customerId, decimal score)
        {
            int area = 0;
            int oldIndex = 0;

            // get area and score of customer
            for (int i = 0; i < 11; i++)
            {
                oldIndex = CacheHandler.CustomerIDList[i].FindIndex(x => x == customerId);

                if (oldIndex != -1)
                {
                    area = i;
                    break;
                }
            }

            decimal oldScore = CacheHandler.ScoreList[area][oldIndex];

            // delete the customer data
            if (area != 11)
                CacheHandler.CustomerRankList.Remove(customerId);

            CacheHandler.CustomerIDList[area].Remove(customerId);
            CacheHandler.ScoreList[area].RemoveAt(oldIndex);

            AddCustomer(customerId, oldScore + score);
        }
    }
}
