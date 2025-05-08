Leaderboard Service
📌 项目简介
该项目实现了一个高性能、基于内存的数据结构的排行榜服务（Leaderboard Service），主要功能包括：

用户分数的动态更新；

基于排名区间查询用户列表；

查询某用户及其前后邻居的排名信息。

该服务使用 线程安全的数据结构（ConcurrentDictionary、SortedSet） 和 读写锁机制（ReaderWriterLockSlim），实现对排行榜的并发读写支持，适合在高并发场景下运行。

🧠 功能理解与实现细节
🔄 UpdateScore(long customerId, decimal scoreDelta)
功能：更新某个客户的分数。

实现逻辑：

使用 ConcurrentDictionary 获取当前分数；

从 SortedSet 中移除旧分数；

如果新分数大于0，重新加入排行榜并更新缓存；

使用写锁保证并发安全；

分数有变动时刷新快照 RefreshSnapshot()。

🧮 GetCustomersByRank(int start, int end)
功能：获取某个排名区间内的用户数据。

实现逻辑：

从 _cachedLeaderboard 快照中读取数据；

利用 Skip 和 Take 实现分页式访问；

返回封装好的 LeaderboardResponse 列表；

快照读取无需加锁，性能优异。

🧍‍♂️ GetCustomerWithNeighbors(long customerId, int high, int low)
功能：获取指定客户及其周围邻居（上下排名）信息。

实现逻辑：

通过 _customers 获取目标客户的排名；

计算目标前后区间的排名范围；

从快照中截取对应区间的数据；

分为 Higher、Current、Lower 三部分返回。

🧹 RefreshSnapshot()
功能：刷新排行榜快照并更新所有客户的排名。

实现逻辑：

将 SortedSet 转换为 List 快照；

使用 Parallel.For 提升排名更新性能；

快照用于后续读操作中的排名查询。

🧰 技术要点
✅ 使用 SortedSet 实现自动排序的排行榜结构；

✅ 用 ConcurrentDictionary 保证多线程环境下的数据一致性；

✅ ReaderWriterLockSlim 控制并发写入安全；

✅ 快照机制 _cachedLeaderboard 支持高性能只读访问；

✅ 并行更新排名优化刷新性能。

单元测试1
