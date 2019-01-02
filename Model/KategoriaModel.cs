using System.Collections.Generic;
using System.Data.SQLite;
using ISBD.Database;

namespace ISBD.Model
{
	public class KategoriaModel: ObservableModel, Database.IDBInsertable, IDBTableItem, IDBSelectable, IDBUpdateable
	{
		public long IdK { get; set; }

		private string _Nazwa;
		public string Nazwa
		{
			get => _Nazwa;
			set
			{
				if (_Nazwa == null || _Nazwa.Equals(value) == false)
				{
					_Nazwa = value;
					NotifyPropertyChanged();
				}
			}
		}

		private int _Rodzaj;
		public int Rodzaj
		{
			get => _Rodzaj;
			set
			{
				if (_Rodzaj != value)
				{
					_Rodzaj = value;
					NotifyPropertyChanged();
				}
			}
		}

		private long? _IdKRodzic;
		public long? IdKRodzic
		{
			get => _IdKRodzic;
			set
			{
				if (_IdKRodzic != value)
				{
					_IdKRodzic = value;
					NotifyPropertyChanged();
				}
			}
		}

		private long _IdS;
		public long IdS
		{
			get => _IdS;
			set
			{
				if (_IdS != value)
				{
					_IdS = value;
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
					new NameValuePair("Nazwa", Nazwa).AddQuotes(),
					new NameValuePair("Rodzaj", Rodzaj.ToString()),
				};
				if (IdKRodzic.HasValue)
				{
					list.Add(new NameValuePair("IdKRodzic", IdKRodzic.ToString()));
				}
				list.Add(new NameValuePair("IdS", IdS.ToString()));
				return list;
			}
		}
		public string Table => "Kategorie";
		public long Index { get => IdK; set => IdK = value; }

		public bool Init(SQLiteDataReader reader)
		{
			if (reader.HasRows == false) return false;
			if (reader.FieldCount != 5) return false;


			IdK = reader.GetInt64(0);
			Nazwa = reader.GetString(1);
			Rodzaj = reader.GetInt32(2);
			IdKRodzic = reader.IsDBNull(3) ? (long?)null : reader.GetInt64(3);
			IdS = reader.GetInt64(4);

			return true;
		}

		public static bool operator ==(KategoriaModel left, KategoriaModel right)
		{
			if (ReferenceEquals(left, right)) return true;
			if (ReferenceEquals(left, null)) return false;
			if (ReferenceEquals(right, null)) return false;
			return left.IdK == right.IdK;
		}

		public static bool operator !=(KategoriaModel left, KategoriaModel right)
		{
			return !(left == right);
		}

		public override string ToString()
		{
			return $"IdK: {IdK} | Nazwa: {Nazwa} | Rodzaj: {Rodzaj} | IdKRodzic: {IdKRodzic} | IdS: {IdS}";
		}

		public string IndexName { get => "IdK"; }
	}
}
