using Leaderboard.Services;

var builder = WebApplication.CreateBuilder(args);

// ��ӷ�������
builder.Services.AddSingleton<ILeaderboard, LeaderboardService>();
builder.Services.AddControllers();

var app = builder.Build();

// ����HTTP����ܵ�
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();