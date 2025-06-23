using Microsoft.AspNetCore.Http.HttpResults;
using RedisCache.Api.Infrastructure;
using RedisCache.Api.Interfaces;
using RedisCache.Api.Model;

namespace RedisCache.Api.Endpoints;

public class TestRedisCache : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapPost("/Create", CreateOrderFromRedisCache)
            .WithName(nameof(CreateOrderFromRedisCache));
        app.MapGet("/GetOrder", GetOrderFromRedisCache).WithName(nameof(GetOrderFromRedisCache));
    }

    private async Task<Results<Ok<string>, BadRequest>> CreateOrderFromRedisCache(Order order,ICacheService cacheService, CancellationToken token)
    {
        var item = new Order
        {
            Id = order.Id,
            OrderNumber = order.OrderNumber,
            OrderDate = order.OrderDate,
        };
        await cacheService.SetAsync(item.Id.ToString(), order,token);
        return TypedResults.Ok(item.OrderNumber);
    }

    private async Task<Results<Ok<Order>, BadRequest<string>>> GetOrderFromRedisCache(int orderId,ICacheService cacheService, CancellationToken token)
    {
        var orders = await cacheService.GetAsync<Order>(orderId.ToString(), token);
        if (orders is null)
        {
            return TypedResults.BadRequest("Order not found");
        }
        return TypedResults.Ok(orders);
    }

}