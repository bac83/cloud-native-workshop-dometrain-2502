using System.Text.Json;
using StackExchange.Redis;

namespace Dometrain.Monolith.Api.Courses;

public class CachedCourseRepository : ICourseRepository
{
    private readonly ICourseRepository _courseRepository;
    private readonly IConnectionMultiplexer _connectionMultiplexer;

    public CachedCourseRepository(ICourseRepository courseRepository, IConnectionMultiplexer connectionMultiplexer)
    {
        _courseRepository = courseRepository;
        _connectionMultiplexer = connectionMultiplexer;
    }

    public async Task<Course?> CreateAsync(Course course)
    {
        var created = await _courseRepository.CreateAsync(course);
        if (created is null)
        {
            return null;
        }

        var db = _connectionMultiplexer.GetDatabase();
        var serializeCourse = JsonSerializer.Serialize(course);
        await db.StringSetAsync($"course:id:{course.Id}", serializeCourse);
        return created;
    }

    public Task<Course?> GetByIdAsync(Guid id)
    {
        return _courseRepository.GetByIdAsync(id);
    }

    public Task<Course?> GetBySlugAsync(string slug)
    {
        return _courseRepository.GetBySlugAsync(slug);
    }

    public Task<IEnumerable<Course>> GetAllAsync(string nameFilter, int pageNumber, int pageSize)
    {
        return _courseRepository.GetAllAsync(nameFilter, pageNumber, pageSize);
    }

    public Task<Course?> UpdateAsync(Course course)
    {
        return _courseRepository.UpdateAsync(course);
    }

    public Task<bool> DeleteAsync(Guid id)
    {
        return _courseRepository.DeleteAsync(id);
    }
}
