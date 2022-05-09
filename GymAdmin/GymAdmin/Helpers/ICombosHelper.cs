using Microsoft.AspNetCore.Mvc.Rendering;

namespace GymAdmin.Helpers
{
    public interface ICombosHelper
    {
        Task<IEnumerable<SelectListItem>> GetComboServicesAsync();
        Task<IEnumerable<SelectListItem>> GetComboSchedulesAsync();
        Task<IEnumerable<SelectListItem>> GetComboUsersAsync();
    }
}
