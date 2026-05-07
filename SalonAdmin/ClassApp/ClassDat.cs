using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SalonAdmin.ClassApp
{
    public class Employee
    {
        public int Id { get; set; }
        public string FullName => $"{Фамилия} {Имя} {Отчество}".Trim();
        public string Фамилия { get; set; }
        public string Имя { get; set; }
        public string Отчество { get; set; }
        public string Телефон { get; set; }
        public int Стаж { get; set; }
        public decimal Рейтинг { get; set; }
        public string Квалификация { get; set; }
        public string Фото { get; set; }
        public string Статус { get; set; } = "Активен";
        public bool Блокировка { get; set; }
    }
    public class Service
    {
        public int Id { get; set; }
        public string Название { get; set; }
        public int Длительность { get; set; }
        public decimal Цена { get; set; }
        public string Категория { get; set; }
        public decimal? Себестоимость { get; set; }
    }
    public class Supply
    {
        public int Id { get; set; }
        public string Название { get; set; }
        public string Ед { get; set; }
        public decimal Остаток { get; set; }
        public DateTime? МинСрок { get; set; }
        public bool Критично => Остаток < 20 || (МинСрок != null && МинСрок < DateTime.Today.AddMonths(1));
    }
    public class Booking
    {
        public int Id { get; set; }
        public string Услуга { get; set; }
        public string Мастер { get; set; }
        public string Клиент { get; set; }
        public DateTime Начало { get; set; }
        public DateTime Конец { get; set; }
        public string Статус { get; set; }
        public bool НеЯвился { get; set; }
        public decimal? Итог { get; set; }
    }

    public static class ClassDaT
    {
        public static readonly string ConnectionString =
            @"Data Source=KAKAMPUKTER\SQLEXPRESS;Initial Catalog=Парикмахерская2;Integrated Security=True;Connect Timeout=30;Encrypt=False";

        public static SqlConnection GetConnection()
        {
            var conn = new SqlConnection(ConnectionString);
            conn.Open();
            return conn;
        }

        public static void ShowError(Exception ex, string context = "")
            => MessageBox.Show($"{context}\n{ex.Message}", "Ошибка БД",
                MessageBoxButton.OK, MessageBoxImage.Error);
    }

    public static class DashboardRepo
    {
        public static int GetTodayBookingsCount()
        {
            using var conn = ClassDaT.GetConnection();
            var cmd = new SqlCommand(@"
            SELECT COUNT(*) FROM Запись 
            WHERE CAST(Дата_начала AS DATE) = CAST(GETDATE() AS DATE) 
              AND Id_статус IN (1,2)", conn);
            return (int)cmd.ExecuteScalar();
        }

        public static string GetFreeMastersSummary()
        {
            using var conn = ClassDaT.GetConnection();
            var cmd = new SqlCommand(@"
            SELECT s.Фамилия + ' ' + LEFT(s.Имя,1) + '.' 
            FROM Сотрудник s
            WHERE s.Статус = N'Активен' AND s.Блокировка_доступа = 0
            EXCEPT
            SELECT s.Фамилия + ' ' + LEFT(s.Имя,1) + '.' 
            FROM Сотрудник s
            JOIN Запись z ON s.Id_сотрудник = z.Id_сотрудник
            WHERE CAST(z.Дата_начала AS DATE) = CAST(GETDATE() AS DATE)
              AND z.Id_статус = 1", conn);
            using var r = cmd.ExecuteReader();
            var list = new System.Collections.Generic.List<string>();
            while (r.Read()) list.Add(r.GetString(0));
            return list.Count == 0 ? "Все заняты" : string.Join(", ", list);
        }

        public static string GetLowStockWarning()
        {
            using var conn = ClassDaT.GetConnection();
            var cmd = new SqlCommand(@"
            SELECT TOP 3 r.Название + ' (' + CAST(SUM(z.Количество_закуплено) AS VARCHAR) + ' ' + r.Объем_ед_измерения + ')'
            FROM Расходник r
            JOIN Закупка z ON r.Id_расходник = z.Id_расходник 
                AND z.Срок_годности_партии >= GETDATE()
            GROUP BY r.Id_расходник, r.Название, r.Объем_ед_измерения
            HAVING SUM(z.Количество_закуплено) < 20
            ORDER BY SUM(z.Количество_закуплено)", conn);
            using var r = cmd.ExecuteReader();
            var warns = new System.Collections.Generic.List<string>();
            while (r.Read()) warns.Add(r.GetString(0));
            return warns.Count == 0 ? "—" : string.Join("\n", warns);
        }

        public static decimal GetWeekRevenue()
        {
            using var conn = ClassDaT.GetConnection();
            var cmd = new SqlCommand(@"
            SELECT ISNULL(SUM(Итоговая_стоимость),0) 
            FROM Запись
            WHERE Дата_окончания >= DATEADD(DAY,-7,CAST(GETDATE() AS DATE))
              AND Id_статус = 2", conn);
            return (decimal)cmd.ExecuteScalar();
        }
    }
}