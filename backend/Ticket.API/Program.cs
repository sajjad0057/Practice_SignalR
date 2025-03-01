using Microsoft.EntityFrameworkCore;
using Ticket.API.Data;
using Ticket.API.Hubs;

var builder = WebApplication.CreateBuilder(args);


//// Adding Configurations for using Sqlite with EF Core
builder.Services.AddDbContext<TicketContext>(opts =>
    opts.UseSqlite(builder.Configuration.GetConnectionString("Database")));


//// configure Distributed Cache with Redis
builder.Services.AddStackExchangeRedisCache(opt =>
{
    opt.Configuration = builder.Configuration.GetConnectionString("Redis");
});


//// configure SignalR with Redis backplane
builder.Services.AddSignalR()
    .AddStackExchangeRedis("localhost:6379", options =>
    {
        options.Configuration.ChannelPrefix = "SignalR";
    });


#region ForConfiguring CORS

builder.Services.AddCors(opt =>
{
    opt.AddPolicy("reactApp", builder =>
    {
        builder.WithOrigins("http://localhost:3000")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

#endregion


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


////For configuring CORS
app.UseCors("reactApp");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapHub<TicketHub>("/ticketHub");

app.MapControllers();


app.Run();
