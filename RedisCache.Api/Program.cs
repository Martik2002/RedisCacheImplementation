using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddStackExchangeRedisCache(options =>
{
    if (!builder.Environment.IsProduction())
    {
        options.Configuration = builder.Configuration["Redis:Endpoint"];
    }
    else
    {
        options.ConfigurationOptions = new ConfigurationOptions
        {
            EndPoints = { builder.Configuration["Redis:Endpoint"]! },
            Password = builder.Configuration["Redis:Password"],
            Ssl = true,
            AbortOnConnectFail = true,
            AllowAdmin = true,
        };
    }

    options.InstanceName = builder.Configuration["Redis:Instance"];
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.Run();
