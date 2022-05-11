using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace NeronkaFromShares
{
    internal class DBDataLoader
    {
        private readonly SqliteConnection _connection;

        private HistoricCandleToDB[] _historicCandles;

        public HistoricCandleToDB[] GetHistoricslCandles => _historicCandles;

        public const string DefultDBName = "TestingDB.db";
        public const string DefultTableName = "Test";

        public DBDataLoader(DateTime startDate, DateTime endDate, string DBName = "TestingDB.db", 
            string tableName = "Test", int minimumValue = 99999)
        {
            if (string.IsNullOrEmpty(DBName))
                DBName = DefultDBName;

            if (string.IsNullOrEmpty(tableName))
                tableName = DefultTableName;

            try
            {
                _connection = new SqliteConnection($"Data Source={DBName}");
                _connection.Open();
                SqliteCommand command = _connection.CreateCommand();
                command.Connection = _connection;
                command.CommandText = $"SELECT * FROM {tableName} WHERE Volume >= {minimumValue} AND" +
                    $" DATE(Time) >= DATE('{startDate.ToString("u")}') AND" +
                    $" DATE(Time) <= DATE('{endDate.ToString("u")}')";

                SqliteDataReader reader = command.ExecuteReader();
                if (reader.HasRows) // если есть данные
                {
                    List<HistoricCandleToDB> historicCandlesList = new List<HistoricCandleToDB>();
                    while (reader.Read())   // построчно считываем данные
                    {
                        var id = reader.GetInt32(0);
                        var Time = reader.GetDateTime(1);
                        var Open = reader.GetFloat(2);
                        var Close = reader.GetFloat(3);
                        var High = reader.GetFloat(4);
                        var Low = reader.GetFloat(5);
                        var Volume = reader.GetInt64(6);

                        historicCandlesList.Add(new HistoricCandleToDB(Time, Open, Close, High, Low, Volume));
                    }

                    _historicCandles = historicCandlesList.ToArray();
                }
            }
            catch (Exception ex) 
            { 
                MessageBox.Show(ex.Message);
            }
        }
    }
}
