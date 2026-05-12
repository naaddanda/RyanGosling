using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;

namespace SalonAdmin.ClassApp;

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

    public static void ShowError(Exception ex, string context) =>
        MessageBox.Show($"{context}: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
}

// ===== Models =====

public sealed class Qualification
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
}

public sealed class Employee
{
    public int Id { get; set; }
    public string Фамилия { get; set; } = "";
    public string Имя { get; set; } = "";
    public string? Отчество { get; set; }
    public string Телефон { get; set; } = "";
    public int Стаж { get; set; }
    public decimal? Рейтинг { get; set; }
    public Qualification? Квалификация { get; set; }
    public string? Фото { get; set; }

    // ✅ Новое: статус из БД
    public int Id_статус { get; set; }
    public string Статус { get; set; }

    public string FullName =>
        string.Join(" ", new[] { Фамилия, Имя, Отчество }.Where(s => !string.IsNullOrWhiteSpace(s)));
}

public sealed class Client
{
    public int Id { get; set; }
    public string Фамилия { get; set; } = "";
    public string Имя { get; set; } = "";
    public string? Отчество { get; set; }
    public string Телефон { get; set; } = "";
    public short? Скидка { get; set; }

    public string FullName =>
        string.Join(" ", new[] { Фамилия, Имя, Отчество }.Where(s => !string.IsNullOrWhiteSpace(s)));
}

public sealed class Service
{
    public int Id { get; set; }
    public string Название { get; set; } = "";
    public int Длительность { get; set; }
    public decimal Цена { get; set; }
    public string? Категория { get; set; }
    public decimal? Себестоимость { get; set; }
}

public sealed class Supply
{
    public int Id { get; set; }
    public string Название { get; set; } = "";
    public string Ед { get; set; } = "";
    public int Остаток { get; set; }
    public DateTime? МинСрок { get; set; }
    public bool Критично =>
        Остаток < 20 || (МинСрок.HasValue && МинСрок.Value.Date < DateTime.Today.AddMonths(1));
}

public sealed class Booking
{
    public int Id { get; set; }
    public string Услуга { get; set; } = "";
    public string Мастер { get; set; } = "";
    public string Клиент { get; set; } = "";
    public DateTime Начало { get; set; }
    public DateTime Конец { get; set; }
    public string Статус { get; set; } = "";
    public decimal? Итог { get; set; }

    // Для inline ComboBox в RecordsPage
    public List<string> Statuses { get; set; } = new();
}

public sealed class Category
{
    public int Id { get; set; }
    public string Название { get; set; } = "";
}

public sealed class Promotion
{
    public int Id { get; set; }
    public string Название { get; set; } = "";
    public int Id_услуга { get; set; }
    public string Услуга { get; set; } = "";
    public string Сезон_год { get; set; } = "";
    public string? Описание { get; set; }
    public short? Скидка_процент { get; set; }

    // Для ComboBox в PromotionsPage
    public List<KeyValuePair<int, string>> Services { get; set; } = new();
}

public sealed class JournalEntry
{
    public int Id { get; set; }
    public string Менеджер { get; set; } = "";
    public string Таблица { get; set; } = "";
    public string Действие { get; set; } = "";
    public DateTime ДатаВремя { get; set; }
}

public sealed class Purchase
{
    public int Id { get; set; }
    public int Id_расходник { get; set; }
    public DateTime Дата_закупки { get; set; }
    public int Количество { get; set; }
    public decimal Стоимость { get; set; }
    public string? Условия_хранения { get; set; }
    public DateTime Срок_годности_партии { get; set; }
}

