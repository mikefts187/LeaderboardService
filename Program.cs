using Leaderboard.Services;

var builder = WebApplication.CreateBuilder(args);

// ��� Swagger ����
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ��ӷ�������
builder.Services.AddSingleton<ILeaderboard, LeaderboardService>();
builder.Services.AddControllers();

var app = builder.Build();

// ���� Swagger �м�������ڿ���������
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// ����HTTP����ܵ�
// app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();