namespace Infrastructure.Dtos
{
    public class GetUserDto
    {
        public int UserId { get; set; }
        public required string Name { get; set; }
        public required string Surname { get; set; }
        public required string Username { get; set; }
        public required string Email { get; set; }
        public required bool IsActive { get; set; }
    }
}
