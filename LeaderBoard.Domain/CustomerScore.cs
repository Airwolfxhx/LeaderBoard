using System;

namespace LeaderBoard.Domain
{
    /// <summary>
    /// customer score entity
    /// </summary>
    public class CustomerScore
    {
        public Int64 CustomerId { get; set; }
        public Decimal Score { get; set; }        
    }
}
