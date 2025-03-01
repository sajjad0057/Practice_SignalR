using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;
using Ticket.API.Data;
using Ticket.API.Hubs;

var builder = WebApplication.CreateBuilder(args);


// Configuration for Sqlite with EF Core
builder.Services.AddDbContext<TicketContext>(opts =>
    opts.UseSqlite(builder.Configuration.GetConnectionString("Database")));


// Redis connection string from configuration (appsettings.json)
var redisConnectionStr = builder.Configuration.GetConnectionString("Redis")!;



// Configure Redis ConnectionMultiplexer
/*
The IConnectionMultiplexer is registered as a singleton service, which is more efficient for Redis since
it uses connection pooling internally. Reusing the same connection acrossservices like SignalR and 
distributed cache improves performance and ensures optimal resource usage
*/
var redisConnection = ConnectionMultiplexer.Connect(redisConnectionStr);
builder.Services.AddSingleton<IConnectionMultiplexer>(redisConnection);



// Configure Distributed Cache with Redis
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = redisConnectionStr;  // Reuse the connection string
});



// Configure SignalR with Redis Backplane
builder.Services.AddSignalR()
    .AddStackExchangeRedis(redisConnectionStr, options =>
    {
        options.Configuration.ChannelPrefix = RedisChannel.Literal("SignalR");
    });



// CORS configuration for React App
builder.Services.AddCors(options =>
{
    options.AddPolicy("reactApp", builder =>
    {
        builder.WithOrigins("http://localhost:3000")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});



builder.Services.AddControllers();



// Add Swagger for API documentation (Development only)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



var app = builder.Build();



// Use Swagger in Development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}



// Configure CORS
app.UseCors("reactApp");

app.UseHttpsRedirection();

app.UseAuthorization();


// Map SignalR Hub
app.MapHub<TicketHub>("/ticketHub");


app.MapControllers();


app.Run();
