using Riaya.Application.Common;
using Riaya.Application.Features.Admin.DTOs;

namespace Riaya.Application.Features.Admin.Interfaces;

public interface IAdminService
{
    Task<PagedResult<AdminUserDto>> GetAllUsersAsync(PaginationParams pagination);
    Task DeleteUserAsync(Guid userId);
    Task<List<AdminUserDto>> GetAllDoctorsAsync();
    Task<PagedResult<AdminBookingDto>> GetAllBookingsAsync(PaginationParams pagination);
    Task<AdminStatsDto> GetStatsAsync();
}
