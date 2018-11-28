using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ISBD.Utils;

namespace ISBD.Database
{
	public class Database: Singleton<Database>, IDisposable
	{
		private const string ADDITIONA_DATA_PATH = "Database\\localData.data";
		private string Path
		{
			get => _path;
			set
			{
				Close();
				_path = value;
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
			Connect();
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

		public (string login, string password) GetLastLoginData()
		{
			string line = null;

			try
			{
				if (File.Exists(ADDITIONA_DATA_PATH) == false) File.Create(ADDITIONA_DATA_PATH);
				using (StreamReader file = new StreamReader(ADDITIONA_DATA_PATH))
				{
					string nextLine;
					while ((nextLine = file.ReadLine()) != null)
					{
						if (nextLine.StartsWith("DOL"))
						{
							line = nextLine;
							break;
						}
					}
				}
			}
			catch (Exception e)
			{
				Console.Error.WriteLine(e.ToString());
			}
			if (line == null) return (null, null);

			var loginPassword = line.Remove(0, 4).Split(';');
			return (loginPassword[0], loginPassword[1]);
		}

		public async void SaveLastLogin(string login, string password)
		{
			await Task.Run(() =>
				{
					bool edited = false;
					if (File.Exists(ADDITIONA_DATA_PATH) == false) File.Create(ADDITIONA_DATA_PATH);
					string[] lines = File.ReadAllLines(ADDITIONA_DATA_PATH);
					for (int i = 0; i < lines.Length; i++)
					{
						if (lines[i].StartsWith("DOL"))
						{
							lines[i] = $"DOL:{login};{password}";
							edited = true;
							break;
						}
					}

					if (edited)
					{
						File.WriteAllLines(ADDITIONA_DATA_PATH, lines);
					}
					else
					{
						File.WriteAllText(ADDITIONA_DATA_PATH, $"DOL:{login};{password}");
					}
				}
			);
		}

		public void Dispose()
		{
			Close();
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
	}
}
