using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;

namespace SalonAdmin.ClassApp;

// ✅ Класс должен быть static
public static class EmployeeRepo
{
    public static List<Employee> GetAll()
    {
        const string sql = @"
        SELECT
            s.Id_сотрудник,
            s.Фамилия, s.Имя, s.Отчество, s.Номер_телефона,
            s.Стаж, s.Рейтинг, s.Название_фото,
            q.Id_квалификация, q.Название,
            st.Id_статус, st.Статус
        FROM Сотрудник s
        JOIN Квалификация q ON q.Id_квалификация = s.Id_квалификация
        JOIN Статус st ON st.Id_статус = s.Id_статус
        ORDER BY s.Фамилия, s.Имя";

        var list = new List<Employee>();
        using var conn = ClassDaT.GetConnection();
        using var cmd = new SqlCommand(sql, conn);
        using var r = cmd.ExecuteReader();
        while (r.Read())
        {
            list.Add(new Employee
            {
                Id = r.GetInt32(0),
                Фамилия = r.IsDBNull(1) ? "" : r.GetString(1),
                Имя = r.IsDBNull(2) ? "" : r.GetString(2),
                Отчество = r.IsDBNull(3) ? null : r.GetString(3),
                Телефон = r.IsDBNull(4) ? "" : r.GetString(4),
                Стаж = r.IsDBNull(5) ? 0 : r.GetInt32(5),
                Рейтинг = r.IsDBNull(6) ? (decimal?)null : r.GetDecimal(6),
                Фото = r.IsDBNull(7) ? null : r.GetString(7),
                Квалификация = new Qualification
                {
                    Id = r.IsDBNull(8) ? 0 : r.GetInt32(8),
                    Name = r.IsDBNull(9) ? "" : r.GetString(9)
                },
                // ✅ Статус из БД
                Id_статус = r.GetInt32(10),
                Статус = r.IsDBNull(11) ? "Активен" : r.GetString(11)
            });
        }
        return list;
    }

    public static Employee? GetById(int id)
    {
        const string sql = @"
        SELECT
            s.Id_сотрудник, s.Фамилия, s.Имя, s.Отчество, s.Номер_телефона,
            s.Стаж, s.Рейтинг, s.Название_фото,
            q.Id_квалификация, q.Название,
            st.Id_статус, st.Статус
        FROM Сотрудник s
        JOIN Квалификация q ON q.Id_квалификация = s.Id_квалификация
        JOIN Статус st ON st.Id_статус = s.Id_статус
        WHERE s.Id_сотрудник = @id";

        using var conn = ClassDaT.GetConnection();
        using var cmd = new SqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@id", id);
        using var r = cmd.ExecuteReader();
        if (!r.Read()) return null;

        return new Employee
        {
            Id = r.GetInt32(0),
            Фамилия = r.IsDBNull(1) ? "" : r.GetString(1),
            Имя = r.IsDBNull(2) ? "" : r.GetString(2),
            Отчество = r.IsDBNull(3) ? null : r.GetString(3),
            Телефон = r.IsDBNull(4) ? "" : r.GetString(4),
            Стаж = r.IsDBNull(5) ? 0 : r.GetInt32(5),
            Рейтинг = r.IsDBNull(6) ? (decimal?)null : r.GetDecimal(6),
            Фото = r.IsDBNull(7) ? null : r.GetString(7),
            Квалификация = new Qualification
            {
                Id = r.IsDBNull(8) ? 0 : r.GetInt32(8),
                Name = r.IsDBNull(9) ? "" : r.GetString(9)
            },
            Id_статус = r.GetInt32(10),
            Статус = r.IsDBNull(11) ? "Активен" : r.GetString(11)
        };
    }

    public static List<Employee> Search(List<Employee> source, string query)
    {
        if (string.IsNullOrWhiteSpace(query))
            return source;

        var q = query.Trim().ToLowerInvariant();
        return source
            .Where(x => (x.FullName ?? "").ToLowerInvariant().Contains(q))
            .ToList();
    }
    public static List<KeyValuePair<int, string>> GetStatuses()
    {
        var list = new List<KeyValuePair<int, string>>();
        using var conn = ClassDaT.GetConnection();
        var cmd = new SqlCommand("SELECT Id_статус, Статус FROM Статус ORDER BY Статус", conn);
        using var r = cmd.ExecuteReader();
        while (r.Read())
        {
            list.Add(new KeyValuePair<int, string>(r.GetInt32(0), r.GetString(1)));
        }
        return list;
    }
    public static void ToggleVacation(int id, bool vacation)
    {
        using var conn = ClassDaT.GetConnection();
        // Находим Id статуса "Отпуск" или "Активен"
        var statusName = vacation ? "Отпуск" : "Активен";
        var cmd = new SqlCommand(
            "SELECT Id_статус FROM Статус WHERE Статус = @name", conn);
        cmd.Parameters.AddWithValue("@name", statusName);

        var result = cmd.ExecuteScalar();
        if (result is int statusId)
        {
            var update = new SqlCommand(
                "UPDATE Сотрудник SET Id_статус = @sid WHERE Id_сотрудник = @id", conn);
            update.Parameters.AddWithValue("@sid", statusId);
            update.Parameters.AddWithValue("@id", id);
            update.ExecuteNonQuery();
        }
    }
    //новое
    public static bool Update(Employee emp)
    {
        try
        {
            using var conn = ClassDaT.GetConnection();
            var cmd = new SqlCommand(@"
            UPDATE Сотрудник 
            SET Фамилия = @fam, Имя = @name, Отчество = @ot,
                Номер_телефона = @tel, Стаж = @stazh, Рейтинг = @rat,
                Id_квалификация = @qual, Название_фото = @photo,
                Id_статус = @status
            WHERE Id_сотрудник = @id", conn);

            cmd.Parameters.AddWithValue("@id", emp.Id);
            cmd.Parameters.AddWithValue("@fam", emp.Фамилия ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@name", emp.Имя ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@ot", string.IsNullOrWhiteSpace(emp.Отчество) ? (object)DBNull.Value : emp.Отчество);
            cmd.Parameters.AddWithValue("@tel", emp.Телефон ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@stazh", emp.Стаж);
            cmd.Parameters.AddWithValue("@rat", emp.Рейтинг ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@qual", emp.Квалификация?.Id ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@photo", string.IsNullOrWhiteSpace(emp.Фото) ? (object)DBNull.Value : emp.Фото);
            cmd.Parameters.AddWithValue("@status", emp.Id_статус); // ✅ Статус

            return cmd.ExecuteNonQuery() > 0;
        }
        catch (Exception ex)
        {
            ClassDaT.ShowError(ex, "EmployeeRepo.Update");
            return false;
        }
    }

    // ✅ Получение списка квалификаций для ComboBox
    public static List<Qualification> GetQualifications()
    {
        var list = new List<Qualification>();
        using var conn = ClassDaT.GetConnection();
        var cmd = new SqlCommand("SELECT Id_квалификация, Название FROM Квалификация ORDER BY Название", conn);
        using var r = cmd.ExecuteReader();
        while (r.Read())
        {
            list.Add(new Qualification
            {
                Id = r.GetInt32(0),
                Name = r.GetString(1)
            });
        }
        return list;
    }
}