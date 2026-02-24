using Business.Entities;
using Services.Dtos.Users;

namespace Services.Mappers;

public static class UserMapper
{
    public static UserDto ToDto(this User user) =>
        new(user.Id, user.Name, user.Email, user.Role, user.Department);
}
