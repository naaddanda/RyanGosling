using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace SalonAdmin.ClassApp;

public sealed class SupplyRepo
{
    public List<Supply> GetAllSupplies()
    {
        // Остаток считаем как сумму Количество_закуплено по партиям с неистекшим сроком.
        // Минимальный срок — ближайшая дата годности среди неистекших партий.
        const string sql = @"
SELECT
    r.Id_расходник,
    r.Название,
    r.Объем_ед_измерения,
    ISNULL(SUM(CASE WHEN z.Срок_годности_партии >= CAST(GETDATE() AS DATE) THEN z.Количество_закуплено ELSE 0 END), 0) AS Qty,
    MIN(CASE WHEN z.Срок_годности_партии >= CAST(GETDATE() AS DATE) THEN z.Срок_годности_партии ELSE NULL END) AS MinExp
FROM Расходник r
LEFT JOIN Закупка z ON z.Id_расходник = r.Id_расходник
GROUP BY r.Id_расходник, r.Название, r.Объем_ед_измерения
ORDER BY r.Название";

        var list = new List<Supply>();
        using var conn = ClassDaT.GetConnection();
        using var cmd = new SqlCommand(sql, conn);
        using var r = cmd.ExecuteReader();
        while (r.Read())
        {
            list.Add(new Supply
            {
                Id = r.GetInt32(0),
                Название = r.IsDBNull(1) ? "" : r.GetString(1),
                Ед = r.IsDBNull(2) ? "" : r.GetString(2),
                Остаток = r.IsDBNull(3) ? 0 : Convert.ToInt32(r.GetValue(3)),
                МинСрок = r.IsDBNull(4) ? (DateTime?)null : Convert.ToDateTime(r.GetValue(4))
            });
        }

        return list;
    }

    public List<Purchase> GetPurchasesForSupply(int supplyId)
    {
        const string sql = @"
SELECT Id_закупка, Id_расходник, Дата_закупки, Количество_закуплено, Стоимость_закупки, Условия_хранения, Срок_годности_партии
FROM Закупка
WHERE Id_расходник = @id
ORDER BY Дата_закупки DESC";

        var list = new List<Purchase>();
        using var conn = ClassDaT.GetConnection();
        using var cmd = new SqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@id", supplyId);
        using var r = cmd.ExecuteReader();
        while (r.Read())
        {
            list.Add(new Purchase
            {
                Id = r.GetInt32(0),
                Id_расходник = r.GetInt32(1),
                Дата_закупки = r.IsDBNull(2) ? DateTime.Today : Convert.ToDateTime(r.GetValue(2)),
                Количество = r.IsDBNull(3) ? 0 : r.GetInt32(3),
                Стоимость = r.IsDBNull(4) ? 0m : r.GetDecimal(4),
                Условия_хранения = r.IsDBNull(5) ? null : r.GetString(5),
                Срок_годности_партии = r.IsDBNull(6) ? DateTime.Today : Convert.ToDateTime(r.GetValue(6))
            });
        }

        return list;
    }
}


