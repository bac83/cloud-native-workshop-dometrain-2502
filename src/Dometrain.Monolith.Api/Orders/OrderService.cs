using Dometrain.Monolith.Api.Enrollments;
using Dometrain.Monolith.Api.Students;

namespace Dometrain.Monolith.Api.Orders;

public interface IOrderService
{
    Task<Order?> GetByIdAsync(Guid orderId);

    Task<IEnumerable<Order>> GetAllForStudentAsync(Guid studentId);

    Task<Order?> PlaceAsync(Guid studentId, IEnumerable<Guid> courseIds);
}

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IEnrollmentService _enrollmentService;
    private readonly IStudentRepository _studentRepository;
    
    public OrderService(IOrderRepository orderRepository, IStudentRepository studentRepository, IEnrollmentService enrollmentService)
    {
        _orderRepository = orderRepository;
        _studentRepository = studentRepository;
        _enrollmentService = enrollmentService;
    }

    public async Task<Order?> GetByIdAsync(Guid orderId)
    {
        return await _orderRepository.GetByIdAsync(orderId);
    }

    public async Task<IEnumerable<Order>> GetAllForStudentAsync(Guid studentId)
    {
        return await _orderRepository.GetAllForStudentAsync(studentId);
    }

    public async Task<Order?> PlaceAsync(Guid studentId, IEnumerable<Guid> courseIds)
    {
        var student = await _studentRepository.GetByIdAsync(studentId);
        if (student is null)
        {
            return null;
        }
        
        var order = await _orderRepository.PlaceAsync(studentId, courseIds);

        if (order is null)
        {
            return null;
        }

        var enrollments = await _enrollmentService.GetStudentEnrollmentsAsync(studentId);
        
        foreach (var courseId in order.CourseIds.Where(x => !enrollments!.CourseIds.Contains(x)))
        {
            await _enrollmentService.EnrollToCourseAsync(studentId, courseId);
        }

        return order;
    }
}
