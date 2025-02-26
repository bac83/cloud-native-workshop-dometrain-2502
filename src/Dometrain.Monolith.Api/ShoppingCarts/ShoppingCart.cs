namespace Dometrain.Monolith.Api.ShoppingCarts;

public class ShoppingCart
{
    public required Guid StudentId { get; set; }

    public List<Guid> CourseIds { get; set; } = [];
}
