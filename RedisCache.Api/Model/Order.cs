namespace RedisCache.Api.Model;

public class Order
{
    public int Id { get; set; }
    public string OrderNumber { get; set; }
    public DateTime OrderDate { get; set; }
}