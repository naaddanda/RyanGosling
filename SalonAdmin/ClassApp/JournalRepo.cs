using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace SalonAdmin.ClassApp;

public sealed class JournalRepo
{
    public List<JournalEntry> GetForRange(DateTime from, DateTime toInclusive)
    {
        const string sql = @"
SELECT
    j.Id_журнал,
    (s.Фамилия + N' ' + LEFT(s.Имя,1) + N'.') AS ManagerShort,
    j.Таблица_изменения,
    j.Действие,
    j.Дата_время
FROM Журнал_действий_менеджера j
JOIN Сотрудник s ON s.Id_сотрудник = j.Id_менеджер
WHERE j.Дата_время >= @from
  AND j.Дата_время < DATEADD(DAY, 1, @to)
ORDER BY j.Дата_время DESC";

        var list = new List<JournalEntry>();
        using var conn = ClassDaT.GetConnection();
        using var cmd = new SqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@from", from);
        cmd.Parameters.AddWithValue("@to", toInclusive.Date);
        using var r = cmd.ExecuteReader();
        while (r.Read())
        {
            list.Add(new JournalEntry
            {
                Id = r.GetInt32(0),
                Менеджер = r.IsDBNull(1) ? "" : r.GetString(1),
                Таблица = r.IsDBNull(2) ? "" : r.GetString(2),
                Действие = r.IsDBNull(3) ? "" : r.GetString(3),
                ДатаВремя = r.IsDBNull(4) ? DateTime.MinValue : r.GetDateTime(4)
            });
        }

        return list;
    }
}

