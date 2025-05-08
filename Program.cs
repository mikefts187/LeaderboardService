using Leaderboard.Services;

var builder = WebApplication.CreateBuilder(args);

// 添加 Swagger 服务
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 添加服务到容器
builder.Services.AddSingleton<ILeaderboard, LeaderboardService>();
builder.Services.AddControllers();

var app = builder.Build();

// 启用 Swagger 中间件（仅在开发环境）
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// 配置HTTP请求管道
// app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();