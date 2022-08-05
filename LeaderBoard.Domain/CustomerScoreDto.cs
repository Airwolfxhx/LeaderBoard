using System;

namespace LeaderBoard.Domain
{
    /// <summary>
    /// Leaderboard
    /// </summary>
    public class CustomerScoreDto
    {
        public Int64 CustomerId { get; set; }
        public Decimal Score { get; set; }
        public Int64 Rank { get; set; }
    }
}
