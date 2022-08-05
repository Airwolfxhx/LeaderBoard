using LeaderBoard.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace LeaderBoard.Infrastructure
{    
    /// <summary>
    /// score comparer for binary search
    /// </summary>
    public class ScoreComparer : IComparer<CustomerScore>
    {
        public int Compare(CustomerScore sourceCustomerScore, CustomerScore objectCustomerScore)
        {
            // source customer does not exist
            if (sourceCustomerScore == null)
                return -1;

            // compare score first. The score was sorted by 'desc'.
            int scoreReturnValue = objectCustomerScore.Score.CompareTo(sourceCustomerScore.Score);

            if (scoreReturnValue != 0)
                return scoreReturnValue;
            else
                return sourceCustomerScore.CustomerId.CompareTo(objectCustomerScore.CustomerId);            
        }
    }
}
