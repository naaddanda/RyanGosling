using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace SalonAdmin.ClassApp;

public sealed class DashboardRepo
{
    public int GetTodayBookingsCount()
    {
        const string sql = @"
SELECT COUNT(*)
FROM Запись
WHERE CAST(Дата_начала AS DATE) = CAST(GETDATE() AS DATE)
  AND Id_статус IN (1,2)";

        using var conn = ClassDaT.GetConnection();
        using var cmd = new SqlCommand(sql, conn);
        return Convert.ToInt32(cmd.ExecuteScalar());
    }

    public List<string> GetFreeMastersSummary()
    {
        const string sql = @"
SELECT (Фамилия + N' ' + LEFT(Имя,1) + N'.') AS MasterShort
FROM Сотрудник
WHERE Id_сотрудник NOT IN (
    SELECT Id_сотрудник
    FROM Запись
    WHERE CAST(Дата_начала AS DATE) = CAST(GETDATE() AS DATE)
      AND Id_статус = 1
)
ORDER BY Фамилия, Имя";

        var list = new List<string>();
        using var conn = ClassDaT.GetConnection();
        using var cmd = new SqlCommand(sql, conn);
        using var r = cmd.ExecuteReader();
        while (r.Read())
            list.Add(r.IsDBNull(0) ? "" : r.GetString(0));
        list.RemoveAll(string.IsNullOrWhiteSpace);
        return list;
    }

    public List<string> GetLowStockWarning()
    {
        const string sql = @"
SELECT TOP 3
    r.Название,
    SUM(z.Количество_закуплено) AS Qty
FROM Расходник r
JOIN Закупка z ON z.Id_расходник = r.Id_расходник
WHERE z.Срок_годности_партии >= CAST(GETDATE() AS DATE)
GROUP BY r.Название
HAVING SUM(z.Количество_закуплено) < 20
ORDER BY SUM(z.Количество_закуплено) ASC";

        var list = new List<string>();
        using var conn = ClassDaT.GetConnection();
        using var cmd = new SqlCommand(sql, conn);
        using var r = cmd.ExecuteReader();
        while (r.Read())
        {
            var name = r.IsDBNull(0) ? "" : r.GetString(0);
            // SUM(int) обычно возвращает int (но драйвер может отдать decimal) — читаем безопасно.
            var qty = r.IsDBNull(1) ? 0 : Convert.ToInt32(r.GetValue(1));
            if (!string.IsNullOrWhiteSpace(name))
                list.Add($"{name} — остаток {qty}");
        }

        return list;
    }

    public decimal GetWeekRevenue()
    {
        const string sql = @"
SELECT ISNULL(SUM(Итоговая_стоимость), 0)
FROM Запись
WHERE Дата_окончания >= DATEADD(DAY, -7, CAST(GETDATE() AS DATE))
  AND Id_статус = 2";

        using var conn = ClassDaT.GetConnection();
        using var cmd = new SqlCommand(sql, conn);
        var obj = cmd.ExecuteScalar();
        return obj == null || obj is DBNull ? 0m : Convert.ToDecimal(obj);
    }
}

