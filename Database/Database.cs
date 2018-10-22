using System.Collections.Generic;
using System.Data.SQLite;
using System.Text;
using ISBD.Utils;

namespace ISBD.Database
{
	class Database: Singleton<Database>
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
		private string _path = "Databse\\Database.sqlite";

		private SQLiteConnection _connection;

		public void Connect()
		{
			_connection = new SQLiteConnection($"Data Source={Path};Version=3;");
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

		public SQLiteDataReader Select(string table)
		{
			var command = _connection.CreateCommand();
			command.CommandText = $"SELECT * FROM {table};";
			return command.ExecuteReader();
		}

		public void Insert(IDBInsertable insertable)
		{
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
				if (i == 0)
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
	}
}
