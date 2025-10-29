namespace EV.AdminService.API.DTOs.Requests
{
    public class UpdateUserRolesRequest
    {
        public List<int> RoleIds { get; set; } = new List<int>();
    }
}
