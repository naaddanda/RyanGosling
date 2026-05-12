using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace SalonAdmin.ClassApp;

public sealed class PromotionRepo
{
    public List<Promotion> GetAll()
    {
        const string sql = @"
SELECT
    p.Id_предложение,
    p.Название,
    p.Id_услуга,
    s.Название AS ServiceName,
    p.Сезон_год,
    p.Описание,
    p.Скидка_процент
FROM Сезонное_предложение p
JOIN Услуга s ON s.Id_услуга = p.Id_услуга
ORDER BY p.Id_предложение DESC";

        var list = new List<Promotion>();
        using var conn = ClassDaT.GetConnection();
        using var cmd = new SqlCommand(sql, conn);
        using var r = cmd.ExecuteReader();
        while (r.Read())
        {
            list.Add(new Promotion
            {
                Id = r.GetInt32(0),
                Название = r.IsDBNull(1) ? "" : r.GetString(1),
                Id_услуга = r.IsDBNull(2) ? 0 : r.GetInt32(2),
                Услуга = r.IsDBNull(3) ? "" : r.GetString(3),
                Сезон_год = r.IsDBNull(4) ? "" : r.GetString(4),
                Описание = r.IsDBNull(5) ? null : r.GetString(5),
                Скидка_процент = r.IsDBNull(6) ? (short?)null : r.GetInt16(6)
            });
        }

        return list;
    }

    public void Update(Promotion p)
    {
        const string sql = @"
UPDATE Сезонное_предложение
SET Название = @n,
    Id_услуга = @sid,
    Сезон_год = @s,
    Описание = @d,
    Скидка_процент = @p
WHERE Id_предложение = @id";

        using var conn = ClassDaT.GetConnection();
        using var cmd = new SqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@id", p.Id);
        cmd.Parameters.AddWithValue("@n", p.Название ?? "");
        cmd.Parameters.AddWithValue("@sid", p.Id_услуга);
        cmd.Parameters.AddWithValue("@s", p.Сезон_год ?? "");
        cmd.Parameters.AddWithValue("@d", (object?)p.Описание ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@p", (object?)p.Скидка_процент ?? DBNull.Value);
        cmd.ExecuteNonQuery();
    }
}

