using Leaderboard.Services;

var builder = WebApplication.CreateBuilder(args);

// 添加服务到容器
builder.Services.AddSingleton<ILeaderboard, LeaderboardService>();
builder.Services.AddControllers();

var app = builder.Build();

// 配置HTTP请求管道
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();