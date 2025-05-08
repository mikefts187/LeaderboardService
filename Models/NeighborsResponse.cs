namespace Leaderboard.Models
{
    public class NeighborsResponse
    {
        public LeaderboardResponse Current { get; set; }
        public List<LeaderboardResponse> Higher { get; set; } = new List<LeaderboardResponse>();
        public List<LeaderboardResponse> Lower { get; set; } = new List<LeaderboardResponse>();
    }
}
