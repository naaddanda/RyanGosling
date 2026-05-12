using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace SalonAdmin.ClassApp;

public static class ReportsRepo
{
    // ✅ Выручка за период (только выполненные записи)
    public static decimal GetRevenueForRange(DateTime from, DateTime to)
    {
        using var conn = ClassDaT.GetConnection();
        var cmd = new SqlCommand(@"
            SELECT ISNULL(SUM(Итоговая_стоимость), 0)
            FROM Запись
            WHERE CAST(Дата_начала AS DATE) >= @from 
              AND CAST(Дата_начала AS DATE) <= @to
              AND Id_статус = 2", conn); // 2 = Выполнено
        cmd.Parameters.AddWithValue("@from", from);
        cmd.Parameters.AddWithValue("@to", to);
        return (decimal)cmd.ExecuteScalar();
    }

    // ✅ Топ услуг по выручке
    public static List<string> GetTopServices(DateTime from, DateTime to, int top = 7)
    {
        var list = new List<string>();
        using var conn = ClassDaT.GetConnection();
        var cmd = new SqlCommand(@"
            SELECT TOP (@top) u.Название + ' — ' + CAST(SUM(z.Итоговая_стоимость) AS VARCHAR) + ' ₽'
            FROM Запись z
            JOIN Услуга u ON z.Id_услуга = u.Id_услуга
            WHERE CAST(z.Дата_начала AS DATE) >= @from 
              AND CAST(z.Дата_начала AS DATE) <= @to
              AND z.Id_статус = 2
            GROUP BY u.Название
            ORDER BY SUM(z.Итоговая_стоимость) DESC", conn);
        cmd.Parameters.AddWithValue("@from", from);
        cmd.Parameters.AddWithValue("@to", to);
        cmd.Parameters.AddWithValue("@top", top);

        using var r = cmd.ExecuteReader();
        while (r.Read()) list.Add(r.GetString(0));
        return list;
    }

    // ✅ Топ мастеров по выручке
    public static List<string> GetTopMasters(DateTime from, DateTime to, int top = 7)
    {
        var list = new List<string>();
        using var conn = ClassDaT.GetConnection();
        var cmd = new SqlCommand(@"
            SELECT TOP (@top) (s.Фамилия + ' ' + LEFT(s.Имя,1) + '.') + ' — ' + CAST(SUM(z.Итоговая_стоимость) AS VARCHAR) + ' ₽'
            FROM Запись z
            JOIN Сотрудник s ON z.Id_сотрудник = s.Id_сотрудник
            WHERE CAST(z.Дата_начала AS DATE) >= @from 
              AND CAST(z.Дата_начала AS DATE) <= @to
              AND z.Id_статус = 2
            GROUP BY s.Фамилия, s.Имя
            ORDER BY SUM(z.Итоговая_стоимость) DESC", conn);
        cmd.Parameters.AddWithValue("@from", from);
        cmd.Parameters.AddWithValue("@to", to);
        cmd.Parameters.AddWithValue("@top", top);

        using var r = cmd.ExecuteReader();
        while (r.Read()) list.Add(r.GetString(0));
        return list;
    }
}