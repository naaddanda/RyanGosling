using SalonAdmin.ClassApp;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

public static class SupplyRepo
{
    public static List<Supply> GetActiveWithFilter(DateTime? minExpire = null)
    {
        var list = new List<Supply>();
        using var conn = Db.GetConnection();
        var sql = @"
            SELECT r.Id_расходник, r.Название, r.Объем_ед_измерения,
                   SUM(z.Количество_закуплено) AS Остаток,
                   MIN(z.Срок_годности_партии) AS МинСрок
            FROM Расходник r
            JOIN Закупка z ON r.Id_расходник = z.Id_расходник
            WHERE z.Срок_годности_партии >= GETDATE()";

        if (minExpire.HasValue)
            sql += $" AND z.Срок_годности_партии <= '{minExpire.Value:yyyy-MM-dd}'";

        sql += " GROUP BY r.Id_расходник, r.Название, r.Объем_ед_измерения";

        var cmd = new SqlCommand(sql, conn);
        using var r = cmd.ExecuteReader();
        while (r.Read())
        {
            list.Add(new Supply
            {
                Id = r.GetInt32(0),
                Название = r.GetString(1),
                Ед = r.GetString(2),
                Остаток = r.GetDecimal(3),
                МинСрок = r.IsDBNull(4) ? null : r.GetDateTime(4)
            });
        }
        return list;
    }

    public static void AddBatch(int supplyId, DateTime date, int qty, decimal cost, string storage, DateTime expire)
    {
        using var conn = Db.GetConnection();
        var cmd = new SqlCommand(@"
            INSERT INTO Закупка (Id_расходник, Дата_закупки, Количество_закуплено, 
                               Стоимость_закупки, Условия_хранения, Срок_годности_партии)
            VALUES (@sid, @dt, @qty, @cost, @stor, @exp)", conn);
        cmd.Parameters.AddWithValue("@sid", supplyId);
        cmd.Parameters.AddWithValue("@dt", date);
        cmd.Parameters.AddWithValue("@qty", qty);
        cmd.Parameters.AddWithValue("@cost", cost);
        cmd.Parameters.AddWithValue("@stor", (object)storage ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@exp", expire);
        cmd.ExecuteNonQuery();
    }
}