using SalonAdmin.ClassApp;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace SalonAdmin.ClassApp
{
    public static class BookingRepo
    {
        public static List<Booking> GetForDate(DateTime date)
        {
            var list = new List<Booking>();
            using var conn = ClassDat.GetConnection(); // ✅ Заменено на ClassDat
            var cmd = new SqlCommand(@"
                SELECT z.Id_запись, u.Название, 
                       s.Фамилия + ' ' + LEFT(s.Имя,1) + '.',
                       c.Фамилия + ' ' + LEFT(c.Имя,1) + '.',
                       z.Дата_начала, z.Дата_окончания, st.Название, 
                       z.Итоговая_стоимость, z.Не_явился
                FROM Запись z
                JOIN Услуга u ON z.Id_услуга = u.Id_услуга
                JOIN Сотрудник s ON z.Id_сотрудник = s.Id_сотрудник
                JOIN Клиент c ON z.Id_клиент = c.Id_клиент
                JOIN Статус_записи st ON z.Id_статус = st.Id_статус
                WHERE CAST(z.Дата_начала AS DATE) = @dt
                ORDER BY z.Дата_начала", conn);

            cmd.Parameters.AddWithValue("@dt", date.Date);

            using var r = cmd.ExecuteReader();
            while (r.Read())
            {
                list.Add(new Booking
                {
                    Id = r.GetInt32(0),
                    Услуга = r.GetString(1),
                    Мастер = r.GetString(2),
                    Клиент = r.GetString(3),
                    Начало = r.GetDateTime(4),
                    Конец = r.GetDateTime(5),
                    Статус = r.GetString(6),
                    Итог = r.IsDBNull(7) ? (decimal?)null : r.GetDecimal(7), // ✅ Явный каст против CS8957
                    НеЯвился = !r.IsDBNull(8) && r.GetBoolean(8)
                });
            }
            return list;
        }

        public static void UpdateStatus(int bookingId, string newStatus, bool noShow = false)
        {
            using var conn = ClassDat.GetConnection(); // ✅ Заменено на ClassDat
            var cmd = new SqlCommand(@"
                UPDATE Запись 
                SET Id_статус = (SELECT Id_статус FROM Статус_записи WHERE Название = @st),
                    Не_явился = @ns
                WHERE Id_запись = @id", conn);
            cmd.Parameters.AddWithValue("@st", newStatus);
            cmd.Parameters.AddWithValue("@ns", noShow);
            cmd.Parameters.AddWithValue("@id", bookingId);
            cmd.ExecuteNonQuery();
        }
    }
}