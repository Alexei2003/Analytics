using Npgsql;

namespace Analytics.Server.Funcs
{
    public static class DataBase
    {
        public static string connString = "Host=localhost;Username=postgres;Database=analytics;Encoding=UTF8";

        //public static NpgsqlConnection Conn { get; set; }

        //public static void Open()
        //{
        //    Conn = new NpgsqlConnection(connString);
        //    Conn.Open();

        //    // Установка кодировки клиента
        //    using (var command = new NpgsqlCommand("SET client_encoding TO 'UTF8';", Conn))
        //    {
        //        command.ExecuteNonQuery();
        //    }
        //}

        //public static void Close()
        //{
        //    Conn.Close();
        //}
    }
}
