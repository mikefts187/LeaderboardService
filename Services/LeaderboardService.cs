using Leaderboard.Models;
using System.Collections.Concurrent;

namespace Leaderboard.Services
{
    public class LeaderboardService : ILeaderboard
    {
        private readonly ConcurrentDictionary<long, (decimal Score, int Rank)> _customers = new();
        private readonly SortedSet<(decimal Score, long CustomerId)> _leaderboard = new(
            Comparer<(decimal Score, long CustomerId)>.Create((a, b) =>
                a.Score != b.Score ? b.Score.CompareTo(a.Score) : a.CustomerId.CompareTo(b.CustomerId)));

        private volatile List<(decimal Score, long CustomerId)> _cachedLeaderboard = new();
        private readonly ReaderWriterLockSlim _lock = new();

        public decimal UpdateScore(long customerId, decimal scoreDelta)
        {
            if (scoreDelta < -1000 || scoreDelta > 1000)
                throw new ArgumentOutOfRangeException(nameof(scoreDelta));

            _lock.EnterWriteLock();
            try
            {
                // Get or add current score
                var currentEntry = _customers.GetOrAdd(customerId, _ => (0, -1));
                var newScore = currentEntry.Score + scoreDelta;

                // Remove old entry if exists
                if (currentEntry.Score > 0)
                {
                    _leaderboard.Remove((currentEntry.Score, customerId));
                }

                // Add new entry if score positive
                if (newScore > 0)
                {
                    _leaderboard.Add((newScore, customerId));
                    _customers[customerId] = (newScore, -1); // Rank will be updated in refresh
                }
                else
                {
                    _customers.TryRemove(customerId, out _);
                }

                // Only refresh if the update could affect rankings
                if (scoreDelta != 0)
                {
                    RefreshSnapshot();
                }

                return newScore;
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        private void RefreshSnapshot()
        {
            var newLeaderboard = _leaderboard.ToList();
            _cachedLeaderboard = newLeaderboard;

            // Update ranks in parallel for large leaderboards
            Parallel.For(0, newLeaderboard.Count, i =>
            {
                var (score, customerId) = newLeaderboard[i];
                _customers[customerId] = (score, i + 1);
            });
        }

        public List<LeaderboardResponse> GetCustomersByRank(int start, int end)
        {
            if (start < 1 || end < start)
                throw new ArgumentException("Invalid rank range");

            var snapshot = _cachedLeaderboard;
            if (start > snapshot.Count)
                return new List<LeaderboardResponse>();

            return snapshot
                .Skip(start - 1)
                .Take(end - start + 1)
                .Select((item, index) => new LeaderboardResponse
                {
                    CustomerId = item.CustomerId,
                    Score = item.Score,
                    Rank = start + index
                })
                .ToList();
        }

        public NeighborsResponse GetCustomerWithNeighbors(long customerId, int high, int low)
        {
            if (!_customers.TryGetValue(customerId, out var customer) || customer.Rank == -1)
                throw new KeyNotFoundException("Customer not found in leaderboard");

            var currentRank = customer.Rank;
            var snapshot = _cachedLeaderboard;

            var start = Math.Max(0, currentRank - 1 - high);
            var end = Math.Min(snapshot.Count - 1, currentRank - 1 + low);

            var neighbors = snapshot
                .Skip(start)
                .Take(end - start + 1)
                .Select((item, index) => new LeaderboardResponse
                {
                    CustomerId = item.CustomerId,
                    Score = item.Score,
                    Rank = start + index + 1
                })
                .ToList();

            var currentIndex = currentRank - 1 - start;

            return new NeighborsResponse
            {
                Current = neighbors[currentIndex],
                Higher = neighbors.Take(currentIndex).ToList(),
                Lower = neighbors.Skip(currentIndex + 1).ToList()
            };
        }
    }
}
