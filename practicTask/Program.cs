using System;
using System.Collections.Generic;
using Npgsql;
using Dapper;

public class User
{
    public int U_ID { get; set; }
    public string U_Name { get; set; } = "default value";
    public string U_Email { get; set; } = "default value";
    public long U_Phone { get; set; }
    public DateTime U_Created { get; set; }
    public string RoleName { get; set; } = "default value";
}

public class Role
{
    public int R_ID { get; set; }
    public string R_Name { get; set; } = "default value";
    // Добавлено новое свойство для описания роли
    public string R_Description { get; set; } = "default value";
    public int UserCount { get; set; }
}

public class UserRole
{
    public int UR_ID { get; set; }
    public int User_ID { get; set; }
    public int Role_ID { get; set; }
    public DateTime AssignmentDate { get; set; }
}

class Program
{
    static void Main(string[] args)
    {
        string connectionString = "Server=localhost;Port=5432;Database=practic;User id = postgres; Password=000";
        using (var connection = new NpgsqlConnection(connectionString))
        {
            // Задача 1: Получение пользователей с их ролями
            var usersWithRoles = connection.Query<User, Role, User>(
                "SELECT * FROM Users JOIN UsersRoles ON Users.U_ID = UsersRoles.User_ID JOIN Roles ON UsersRoles.Role_ID = Roles.R_ID",
                (user, role) =>
                {
                    user.RoleName = role.R_Name; // Теперь RoleName есть у класса User
                    return user;
                },
                splitOn: "R_ID"
            );

            // Задача 2: Получение количества пользователей для каждой роли
            var roleUserCounts = connection.Query<Role>(
                "SELECT Roles.R_ID, Roles.R_Name, COUNT(UsersRoles.User_ID) AS UserCount FROM Roles JOIN UsersRoles ON Roles.R_ID = UsersRoles.Role_ID GROUP BY Roles.R_ID, Roles.R_Name"
            );

            // Вывод результатов
            foreach (var user in usersWithRoles)
            {
                Console.WriteLine($"Пользователь: {user.U_Name}, Роль: {user.RoleName}");
            }

            foreach (var role in roleUserCounts)
            {
                Console.WriteLine($"Роль: {role.R_Name}, Количество пользователей: {role.UserCount}");
            }
        }
    }
}
