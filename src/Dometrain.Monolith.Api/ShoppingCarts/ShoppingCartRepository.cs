using System.Net;
using Dapper;
using Dometrain.Monolith.Api.Database;
using Microsoft.Azure.Cosmos;

namespace Dometrain.Monolith.Api.ShoppingCarts;

public class ShoppingCartItemDto
{
    public required Guid StudentId { get; init; }
    public required Guid CourseId { get; init; }
}

public interface IShoppingCartRepository
{
    Task<bool> AddCourseAsync(Guid studentId, Guid courseId);

    Task<ShoppingCart?> GetByIdAsync(Guid studentId);
    
    Task<bool> RemoveItemAsync(Guid studentId, Guid courseId);
    
    Task<bool> ClearAsync(Guid studentId);
}

public class ShoppingCartRepository : IShoppingCartRepository
{
    private readonly CosmosClient _cosmosClient;
    private const string DatabaseId = "cartdb";
    private const string ContainerId = "carts";

    public ShoppingCartRepository(CosmosClient cosmosClient)
    {
        _cosmosClient = cosmosClient;
    }

    public async Task<bool> AddCourseAsync(Guid studentId, Guid courseId)
    {
        var container = _cosmosClient.GetContainer(DatabaseId, ContainerId);
        ShoppingCart cart;

        try
        {
            var cartResponse =
                await container.ReadItemAsync<ShoppingCart>(studentId.ToString(),
                    new PartitionKey(studentId.ToString()));
            cart = cartResponse.Resource;
        }
        catch (CosmosException exception) when (exception.StatusCode == HttpStatusCode.NotFound)
        {
            cart = new ShoppingCart
            {
                StudentId = studentId,
                CourseIds = []
            };
        }

        if (!cart.CourseIds.Contains(courseId))
        {
            cart.CourseIds.Add(courseId);
        }

        var response = await container.UpsertItemAsync(cart);

        return response.StatusCode is HttpStatusCode.OK or HttpStatusCode.Created;
    }

    public async Task<ShoppingCart?> GetByIdAsync(Guid studentId)
    {
        return null;
    }

    public async Task<bool> RemoveItemAsync(Guid studentId, Guid courseId)
    {
        return true;
    }

    public async Task<bool> ClearAsync(Guid studentId)
    {
        return true;
    }
}
