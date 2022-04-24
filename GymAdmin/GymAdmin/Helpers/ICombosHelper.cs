using Microsoft.AspNetCore.Mvc.Rendering;

namespace GymAdmin.Helpers
{
    public interface ICombosHelper
    {
        Task<IEnumerable<SelectListItem>> GetComboProfessionalsAsync();
        Task<IEnumerable<SelectListItem>> GetComboSchedulesAsync();
        Task<IEnumerable<SelectListItem>> GetComboUsersAsync();
    }
}
