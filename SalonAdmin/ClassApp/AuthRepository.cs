using System;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace SalonAdmin.ClassApp
{
    public class User
    {
        public int Id { get; set; }
        public string Login { get; set; } = "";
        public int RoleId { get; set; }
        public string RoleName { get; set; } = "";
    }

    public static class AuthRepository
    {
        public static async Task<User?> AuthenticateAsync(string login, string password)
        {
            const string query = @"
                SELECT u.Id_пользователь, u.Логин, u.Пароль_hash, u.Id_роль, r.Название AS RoleName
                FROM Пользователь u
                INNER JOIN Роль r ON u.Id_роль = r.Id_роль
                WHERE u.Логин = @login";

            using (var conn = ClassDaT.GetConnection())
            using (var cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@login", login);
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        string? hashFromDb = reader["Пароль_hash"]?.ToString();
                        // Сравниваем введённый пароль с тем, что в БД (без изменений)
                        if (password.Trim() == hashFromDb)
                        {
                            return new User
                            {
                                Id = Convert.ToInt32(reader["Id_пользователь"]),
                                Login = reader["Логин"].ToString() ?? "",
                                RoleId = Convert.ToInt32(reader["Id_роль"]),
                                RoleName = reader["RoleName"].ToString() ?? ""
                            };
                        }
                    }
                }
            }
            return null;
        }
    }
}