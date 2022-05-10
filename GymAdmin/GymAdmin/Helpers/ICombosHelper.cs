using Microsoft.AspNetCore.Mvc.Rendering;

namespace GymAdmin.Helpers
{
    public interface ICombosHelper
    {
        Task<IEnumerable<SelectListItem>> GetComboServicesAsync();
        Task<IEnumerable<SelectListItem>> GetComboSchedulesAsync(int serviceId, DayOfWeek day);
        Task<IEnumerable<SelectListItem>> GetComboUsersAsync();
    }
}
