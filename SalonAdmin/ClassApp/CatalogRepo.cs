using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace SalonAdmin.ClassApp;

public sealed class CatalogRepo
{
    public List<Category> GetServiceCategories()
    {
        const string sql = @"SELECT Id_категория, Название FROM Категория_услуг ORDER BY Id_категория";
        var list = new List<Category>();
        using var conn = ClassDaT.GetConnection();
        using var cmd = new SqlCommand(sql, conn);
        using var r = cmd.ExecuteReader();
        while (r.Read())
        {
            list.Add(new Category
            {
                Id = r.GetInt32(0),
                Название = r.IsDBNull(1) ? "" : r.GetString(1)
            });
        }
        return list;
    }

    public Dictionary<int, string> GetServicesMap()
    {
        const string sql = @"SELECT Id_услуга, Название FROM Услуга ORDER BY Название";
        var dict = new Dictionary<int, string>();
        using var conn = ClassDaT.GetConnection();
        using var cmd = new SqlCommand(sql, conn);
        using var r = cmd.ExecuteReader();
        while (r.Read())
        {
            var id = r.GetInt32(0);
            var name = r.IsDBNull(1) ? "" : r.GetString(1);
            if (!dict.ContainsKey(id))
                dict[id] = name;
        }
        return dict;
    }
}

