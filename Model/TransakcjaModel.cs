using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SQLite;
using System.Runtime.CompilerServices;
using ISBD.Database;

namespace ISBD.Model
{
	public class TransakcjaModel : ObservableModel, Database.IDBInsertable, IDBTableItem, IDBSelectable, IDBUpdateable
	{
		public static List<TransakcjaModel> FromCSV(string csv)
		{
			string[] lines = csv.Split('\n');
			if (lines.Length < 1)
			{
				return null;
			}

			List<TransakcjaModel> transactions = new List<TransakcjaModel>(lines.Length);

			foreach (string line in lines)
			{
				string[] splitted = line.Split(';');
				if (splitted.Length != 3)
				{
					continue;
				}

				DateTime date = new DateTime(long.Parse(splitted[0]));
				long idK = long.Parse(splitted[1]);
				double amount = double.Parse(splitted[2]);

				TransakcjaModel transaction = new TransakcjaModel()
					{_Data = date, _IdK = idK, _IdO = 1, _Kwota = amount, _Tytul = "Tytuł", _Opis = ""};
				transactions.Add(transaction);
			}

			return transactions;
		}

		public long IdT { get; set; }

		private double _Kwota;
		public double Kwota
		{
			get => _Kwota;
			set
			{
				if (_Kwota != value)
				{
					_Kwota = value;
					NotifyPropertyChanged();
				}
			}
		}

		private string _Tytul;
		public string Tytul
		{
			get => _Tytul;
			set
			{
				if (_Tytul == null || _Tytul.Equals(value) == false)
				{
					_Tytul = value;
					NotifyPropertyChanged();
				}
			}
		}

		private string _Opis;
		public string Opis
		{
			get => _Opis;
			set
			{
				if (_Opis == null || _Opis.Equals(value) == false)
				{
					_Opis = value;
					NotifyPropertyChanged();
				}
			}
		}

		private DateTime _Data;
		public DateTime Data
		{
			get => _Data;
			set
			{
				if (_Data != value)
				{
					_Data = value;
					NotifyPropertyChanged();
				}
			}
		}

		private long _IdO;
		public long IdO
		{
			get => _IdO;
			set
			{
				if (_IdO != value)
				{
					_IdO = value;
					NotifyPropertyChanged();
				}
			}
		}

		private long _IdK;
		public long IdK
		{
			get => _IdK;
			set
			{
				if (_IdK != value)
				{
					_IdK = value;
					NotifyPropertyChanged();
				}
			}
		}

		public IList<NameValuePair> NamedValues
		{
			get
			{
				var list = new List<NameValuePair>()
				{
					new NameValuePair("Kwota", Kwota.ToString()),
					new NameValuePair("Tytul", Tytul).AddQuotes(),
				};
				if (string.IsNullOrEmpty(Opis) == false)
				{
					list.Add(new NameValuePair("Opis", Opis).AddQuotes());
				}
				list.Add(new NameValuePair("Data", Data.Ticks.ToString()));
				list.Add(new NameValuePair("IdO", IdO.ToString()));
				list.Add(new NameValuePair("IdK", IdK.ToString()));
				return list;
			}
		}

		public string Table => "Transakcje";

		public string IndexName => "IdT";
		public long Index
		{
			get => IdT;
			set => IdT = value;
		}

		public bool Init(SQLiteDataReader reader)
		{
			if (reader.HasRows == false) return false;
			if (reader.FieldCount != 7) return false;

			IdT = reader.GetInt64(0);
			Kwota = reader.GetDouble(1);
			Tytul = reader.GetString(2);
			Opis = reader.IsDBNull(3) ? "" : reader.GetString(3);
			Data = new DateTime(reader.GetInt64(4));
			IdO = reader.GetInt64(5);
			IdK = reader.GetInt64(6);

			return true;
		}
		public static bool operator ==(TransakcjaModel left, TransakcjaModel right)
		{
			if (ReferenceEquals(left, right)) return true;
			if (ReferenceEquals(left, null)) return false;
			if (ReferenceEquals(right, null)) return false;
			return left.IdT == right.IdT;
		}

		public static bool operator !=(TransakcjaModel left, TransakcjaModel right)
		{
			return !(left == right);
		}

		public override string ToString()
		{
			return $"IdT: {IdT} | Kwota: {Kwota} | Tytul: {Tytul} | Opis: {Opis} | Data: {Data} | IdO: {IdO} | IdK: {IdK}";
		}
	}
}
