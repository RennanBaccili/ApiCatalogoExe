namespace ApiCatalogo.DTOs.AuthenticationsDTO
{
    public class UserWithRolesDto
    {
        public string UserId { get; set; }
        public string UserName { get; set; }

        public string Email { get; set; }
        public IList<string> Roles { get; set; }
    }
}
