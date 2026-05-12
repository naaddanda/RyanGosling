using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace SalonAdmin.ClassApp;

public sealed class ServiceRepo
{
    public List<Service> GetAll()
    {
        const string sql = @"
SELECT
    s.Id_услуга,
    s.Название,
    s.Длительность_мин,
    s.Стоимость,
    c.Название AS CategoryName
FROM Услуга s
JOIN Категория_услуг c ON c.Id_категория = s.Id_категория
ORDER BY c.Название, s.Название";

        var list = new List<Service>();
        using var conn = ClassDaT.GetConnection();
        using var cmd = new SqlCommand(sql, conn);
        using var r = cmd.ExecuteReader();
        while (r.Read())
        {
            list.Add(new Service
            {
                Id = r.GetInt32(0),
                Название = r.IsDBNull(1) ? "" : r.GetString(1),
                Длительность = r.IsDBNull(2) ? 0 : r.GetInt32(2),
                Цена = r.IsDBNull(3) ? 0m : r.GetDecimal(3),
                Категория = r.IsDBNull(4) ? null : r.GetString(4)
            });
        }
        return list;
    }

    public List<Service> Filter(List<Service> source, string category, string query)
    {
        var q = source.AsEnumerable();
        if (!string.IsNullOrWhiteSpace(category) && category != "Все категории")
            q = q.Where(x => string.Equals(x.Категория, category, StringComparison.OrdinalIgnoreCase));
        if (!string.IsNullOrWhiteSpace(query))
        {
            var s = query.Trim().ToLowerInvariant();
            q = q.Where(x => (x.Название ?? "").ToLowerInvariant().Contains(s));
        }
        return q.ToList();
    }

    public void Update(Service s)
    {
        const string sql = @"
UPDATE Услуга
SET Название = @n,
    Длительность_мин = @d,
    Стоимость = @p
WHERE Id_услуга = @id";

        using var conn = ClassDaT.GetConnection();
        using var cmd = new SqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@id", s.Id);
        cmd.Parameters.AddWithValue("@n", s.Название ?? "");
        cmd.Parameters.AddWithValue("@d", s.Длительность);
        cmd.Parameters.AddWithValue("@p", s.Цена);
        cmd.ExecuteNonQuery();
    }
}


