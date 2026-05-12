using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace SalonAdmin.ClassApp;

public sealed class ClientRepo
{
    public List<Client> GetAll()
    {
        const string sql = @"
SELECT Id_клиент, Фамилия, Имя, Отчество, Номер_телефона, Процент_скидки
FROM Клиент
ORDER BY Фамилия, Имя";

        var list = new List<Client>();
        using var conn = ClassDaT.GetConnection();
        using var cmd = new SqlCommand(sql, conn);
        using var r = cmd.ExecuteReader();
        while (r.Read())
        {
            list.Add(new Client
            {
                Id = r.GetInt32(0),
                Фамилия = r.IsDBNull(1) ? "" : r.GetString(1),
                Имя = r.IsDBNull(2) ? "" : r.GetString(2),
                Отчество = r.IsDBNull(3) ? null : r.GetString(3),
                Телефон = r.IsDBNull(4) ? "" : r.GetString(4),
                Скидка = r.IsDBNull(5) ? (short?)null : r.GetInt16(5)
            });
        }

        return list;
    }

    public List<Client> Search(List<Client> source, string query)
    {
        if (string.IsNullOrWhiteSpace(query))
            return source;

        var q = query.Trim().ToLowerInvariant();
        return source.Where(x =>
                (x.FullName ?? "").ToLowerInvariant().Contains(q) ||
                (x.Телефон ?? "").ToLowerInvariant().Contains(q))
            .ToList();
    }

    public void Update(Client c)
    {
        const string sql = @"
UPDATE Клиент
SET Фамилия = @ln,
    Имя = @fn,
    Отчество = @mn,
    Номер_телефона = @ph,
    Процент_скидки = @ds
WHERE Id_клиент = @id";

        using var conn = ClassDaT.GetConnection();
        using var cmd = new SqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@id", c.Id);
        cmd.Parameters.AddWithValue("@ln", c.Фамилия ?? "");
        cmd.Parameters.AddWithValue("@fn", c.Имя ?? "");
        cmd.Parameters.AddWithValue("@mn", (object?)c.Отчество ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@ph", c.Телефон ?? "");
        cmd.Parameters.AddWithValue("@ds", (object?)c.Скидка ?? DBNull.Value);
        cmd.ExecuteNonQuery();
    }
}

