using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Globalization;
using System.Text;
using System.Threading;
using ISBD.Utils;

namespace ISBD.Database
{
	public class Database: Singleton<Database>, IDisposable
	{
		private string Path
		{
			get => _path;
			set
			{
				this.Close();
				_path = value;
				this.Connect();
			}
		}
		private string _path = "Database\\Database.sqlite";

		private SQLiteConnection _connection;

		public void Connect()
		{
			_connection = new SQLiteConnection($"Data Source={/*AppDomain.CurrentDomain.BaseDirectory+*/Path};Version=3;");
			_connection.Open();
		}
		public void Connect(string path)
		{
			Path = path;
		}

		public void Close()
		{
			_connection?.Close();
		}

		public List<T> SelectAll<T>() where T : IDBTableItem, IDBSelectable, new()
		{
			Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
			T item = new T();
			List<T> returnList = new List<T>();

			var reader = SelectAll(item.Table);
			while (reader.Read())
			{
				if (!item.Init(reader)) continue;
				returnList.Add(item);
				item = new T();
			}

			return returnList;
		}

		public void Insert<T>(T insertable) where T: IDBTableItem, IDBInsertable
		{
			Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
			var command = _connection.CreateCommand();
			command.CommandText = $"INSERT INTO {insertable.Table} {GetInsertSQL(insertable)};";
			command.ExecuteNonQuery();
		}

		private string GetInsertSQL(IDBInsertable insertable)
		{
			StringBuilder names = new StringBuilder("(");
			StringBuilder values = new StringBuilder("(");
			IList<NameValuePair> namedValuePairs = insertable.NamedValues;
			for (int i = 0; i < namedValuePairs.Count; i++)
			{
				if (i != 0)
				{
					names.Append(", ");
					values.Append(", ");
				}

				names.Append(namedValuePairs[i].Name);
				values.Append(namedValuePairs[i].Value);
			}

			names.Append(")");
			values.Append(")");
			return $"{names.ToString()} VALUES {values.ToString()}";
		}

		private SQLiteDataReader SelectAll(string table)
		{
			var command = _connection.CreateCommand();
			command.CommandText = $"SELECT * FROM {table};";
			return command.ExecuteReader();
		}

		public void Dispose()
		{
			Close();
		}
	}
}
