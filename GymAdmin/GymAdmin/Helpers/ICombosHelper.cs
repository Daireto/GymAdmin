using Microsoft.AspNetCore.Mvc.Rendering;

namespace GymAdmin.Helpers
{
    public interface ICombosHelper
    {
        Task<IEnumerable<SelectListItem>> GetComboEventsAsync();
        Task<IEnumerable<SelectListItem>> GetComboProfessionalsAsync();
        Task<IEnumerable<SelectListItem>> GetComboDirectorsAsync();
        Task<IEnumerable<SelectListItem>> GetComboSchedulesAsync();
        Task<IEnumerable<SelectListItem>> GetComboUsersAsync();
        Task<IEnumerable<SelectListItem>> GetComboUsersWithEventAsync();
    }
}
