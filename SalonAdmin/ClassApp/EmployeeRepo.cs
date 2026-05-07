using SalonAdmin.ClassApp;
using System.Collections.Generic;
using System.Data.SqlClient;

public static class EmployeeRepo
{
    public static List<Employee> GetAll()
    {
        var list = new List<Employee>();
        using var conn = Db.GetConnection();
        var cmd = new SqlCommand(@"
            SELECT s.Id_сотрудник, s.Фамилия, s.Имя, s.Отчество, s.Номер_телефона, 
                   s.Стаж, s.Рейтинг, s.Название_фото, k.Название AS Квалификация,
                   s.Статус, s.Блокировка_доступа
            FROM Сотрудник s
            JOIN Квалификация k ON s.Id_квалификация = k.Id_квалификация", conn);
        using var r = cmd.ExecuteReader();
        while (r.Read())
        {
            list.Add(new Employee
            {
                Id = r.GetInt32(0),
                Фамилия = r.GetString(1),
                Имя = r.GetString(2),
                Отчество = r.IsDBNull(3) ? "" : r.GetString(3),
                Телефон = r.GetString(4),
                Стаж = r.GetInt32(5),
                Рейтинг = r.GetDecimal(6),
                Фото = r.IsDBNull(8) ? "" : r.GetString(8),
                Квалификация = r.GetString(9),
                Статус = r.IsDBNull(10) ? "Активен" : r.GetString(10),
                Блокировка = !r.IsDBNull(11) && r.GetBoolean(11)
            });
        }
        return list;
    }

    public static void ToggleVacation(int empId, bool isVacation)
    {
        using var conn = Db.GetConnection();
        var cmd = new SqlCommand(
            "UPDATE Сотрудник SET Статус = @st WHERE Id_сотрудник = @id", conn);
        cmd.Parameters.AddWithValue("@st", isVacation ? "Отпуск" : "Активен");
        cmd.Parameters.AddWithValue("@id", empId);
        cmd.ExecuteNonQuery();
    }

    public static void ToggleBlock(int empId, bool block)
    {
        using var conn = Db.GetConnection();
        var cmd = new SqlCommand(
            "UPDATE Сотрудник SET Блокировка_доступа = @b WHERE Id_сотрудник = @id", conn);
        cmd.Parameters.AddWithValue("@b", block);
        cmd.Parameters.AddWithValue("@id", empId);
        cmd.ExecuteNonQuery();
    }
}