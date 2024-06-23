using System.Diagnostics;
using RedisNaruto;
using RedisNaruto.Enums;
using RedisNaruto.Models;
using StackExchange.Redis;
using test.aspnetcpre;
using ServerType = RedisNaruto.Enums.ServerType;

ThreadPool.SetMaxThreads(200, 200);
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// //注册
// var redis = await RedisConnection.CreateAsync(
//     "127.0.0.1:55002,127.0.0.1:55000,servertype=2,username=default,database=5,password=redispw,poolcount=5");

//注册
var build = new ConnectionBuilder
{
    Connection = new string[]
    {
        "127.0.0.1:56379"
    },
    DataBase = 0,
};
build.UseClientSideCaching(new ClientSideCachingOption
{
    Mode = ClientSideCachingModeEnum.TRACKING,
    KeyPrefix = new string[]
    {
        "test",
        "hello"
    },
    TimeOut = default,
    Capacity = 0
});
var redis = await RedisConnection.CreateAsync(build);


builder.Services.AddSingleton(redis);
DiagnosticListener.AllListeners.Subscribe(new RedisNarutoListenerAdapter_Case1());


// ConfigurationOptions configurationOptions = ConfigurationOptions.Parse(
//     "127.0.0.1:55000,password=redispw,connectTimeout=30000,asyncTimeout=30000,syncTimeout=30000,defaultDatabase=1");
// var re = await (ConnectionMultiplexer.ConnectAsync(configurationOptions));
// builder.Services.AddSingleton(re);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();