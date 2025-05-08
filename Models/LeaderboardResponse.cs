namespace Leaderboard.Models
{
    public class LeaderboardResponse
    {
        public long CustomerId { get; set; }
        public decimal Score { get; set; }
        public int Rank { get; set; }
    }
}
