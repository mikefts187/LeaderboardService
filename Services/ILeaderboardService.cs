using Leaderboard.Models;

namespace Leaderboard.Services
{
    public interface ILeaderboard
    {
        decimal UpdateScore(long customerId, decimal scoreDelta);
        List<LeaderboardResponse> GetCustomersByRank(int start, int end);
        NeighborsResponse GetCustomerWithNeighbors(long customerId, int high, int low);
    }
}
