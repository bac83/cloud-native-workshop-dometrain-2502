using Dapper;
using Dometrain.Monolith.Api.Database;

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
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public ShoppingCartRepository(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task<bool> AddCourseAsync(Guid studentId, Guid courseId)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync();
        
        var result = await connection.ExecuteAsync(
            """
            insert into shopping_cart_items (student_id, course_id)
            values (@studentId, @courseId) on conflict do nothing
            """, new { studentId, courseId });

        return result > 0;
    }

    public async Task<ShoppingCart?> GetByIdAsync(Guid studentId)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync();
        var shoppingCart = await connection.QueryAsync<ShoppingCartItemDto>(
            "select student_id StudentId, course_id CourseId from shopping_cart_items where student_id = @studentId", new { studentId });

        return new ShoppingCart
        {
            StudentId = studentId,
            CourseIds = shoppingCart.Select(x => x.CourseId).ToList()
        };
    }

    public async Task<bool> RemoveItemAsync(Guid studentId, Guid courseId)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync();
        var result = await connection.ExecuteAsync(
            """
            delete from shopping_cart_items
            where student_id = @studentId and course_id = @courseId
            """, new { studentId, courseId });

        return result > 0;
    }

    public async Task<bool> ClearAsync(Guid studentId)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync();
        var result = await connection.ExecuteAsync(
            "delete from shopping_cart_items where student_id = @studentId", new { studentId });
        return result > 0;
    }
}
