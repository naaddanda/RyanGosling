using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace SalonAdmin.ClassApp;

public sealed class BookingRepo
{
    public List<Booking> GetForDate(DateTime date)
    {
        const string sql = @"
SELECT
    b.Id_запись,
    srv.Название AS ServiceName,
    (emp.Фамилия + N' ' + LEFT(emp.Имя,1) + N'.') AS MasterShort,
    (cl.Фамилия + N' ' + LEFT(cl.Имя,1) + N'.') AS ClientShort,
    b.Дата_начала,
    b.Дата_окончания,
    st.Название AS StatusName,
    b.Итоговая_стоимость
FROM Запись b
JOIN Услуга srv ON srv.Id_услуга = b.Id_услуга
JOIN Сотрудник emp ON emp.Id_сотрудник = b.Id_сотрудник
JOIN Клиент cl ON cl.Id_клиент = b.Id_клиент
JOIN Статус_записи st ON st.Id_статус = b.Id_статус
WHERE CAST(b.Дата_начала AS DATE) = @dt
ORDER BY b.Дата_начала";

        var list = new List<Booking>();
        using var conn = ClassDaT.GetConnection();
        using var cmd = new SqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@dt", date.Date);
        using var r = cmd.ExecuteReader();
        while (r.Read())
        {
            list.Add(new Booking
            {
                Id = r.GetInt32(0),
                Услуга = r.IsDBNull(1) ? "" : r.GetString(1),
                Мастер = r.IsDBNull(2) ? "" : r.GetString(2),
                Клиент = r.IsDBNull(3) ? "" : r.GetString(3),
                Начало = r.IsDBNull(4) ? date.Date : r.GetDateTime(4),
                Конец = r.IsDBNull(5) ? date.Date : r.GetDateTime(5),
                Статус = r.IsDBNull(6) ? "" : r.GetString(6),
                Итог = r.IsDBNull(7) ? (decimal?)null : r.GetDecimal(7)
            });
        }

        return list;
    }

    public void UpdateStatus(int bookingId, string newStatus)
    {
        const string sql = @"
UPDATE Запись
SET Id_статус = (SELECT TOP 1 Id_статус FROM Статус_записи WHERE Название = @st)
WHERE Id_запись = @id";

        using var conn = ClassDaT.GetConnection();
        using var cmd = new SqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@st", newStatus);
        cmd.Parameters.AddWithValue("@id", bookingId);
        cmd.ExecuteNonQuery();
    }

    public List<string> GetStatuses()
    {
        const string sql = @"SELECT Название FROM Статус_записи ORDER BY Id_статус";
        var list = new List<string>();
        using var conn = ClassDaT.GetConnection();
        using var cmd = new SqlCommand(sql, conn);
        using var r = cmd.ExecuteReader();
        while (r.Read())
            list.Add(r.IsDBNull(0) ? "" : r.GetString(0));
        return list.Where(s => !string.IsNullOrWhiteSpace(s)).ToList();
    }

    public List<string> GetMasters()
    {
        const string sql = @"
SELECT (Фамилия + N' ' + LEFT(Имя,1) + N'.') AS MasterShort
FROM Сотрудник
ORDER BY Фамилия, Имя";

        var list = new List<string>();
        using var conn = ClassDaT.GetConnection();
        using var cmd = new SqlCommand(sql, conn);
        using var r = cmd.ExecuteReader();
        while (r.Read())
            list.Add(r.IsDBNull(0) ? "" : r.GetString(0));
        return list.Where(s => !string.IsNullOrWhiteSpace(s)).ToList();
    }
}

