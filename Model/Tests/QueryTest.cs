using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Media;
using ISBD.Database;

namespace ISBD.Model.Tests
{
	public class QueryTest
	{
		List<Action> testActions => new List<Action>()
		{
			InsertData
		};

		public QueryTest()
		{
			testActions.ForEach(action => action());
		}

		private void InsertData()
		{
			InsertUprawnienia();
			InsertSymbole();
			InsertKategorie();
			InsertTransakcje();
		}

		private void InsertUprawnienia()
		{
			Database.Database.Instance.Connect();
			List<UprawnienieModel> uprawnienia = new List<UprawnienieModel>()
			{
				new UprawnienieModel {IdOBene = 1, IdOD = 1, Poziom = 0},
				new UprawnienieModel {IdOBene = 1, IdOD = 2, Poziom = -2},
				new UprawnienieModel {IdOBene = 2, IdOD = 2, Poziom = 0},
				new UprawnienieModel {IdOBene = 2, IdOD = 3, Poziom = -1},
				new UprawnienieModel {IdOBene = 3, IdOD = 5, Poziom = 1},
				new UprawnienieModel {IdOBene = 4, IdOD = 5, Poziom = 1},
			};

			uprawnienia.ForEach(uprawnienie => Database.Database.Instance.Insert(uprawnienie));
			Database.Database.Instance.Dispose();
		}

		private void InsertSymbole()
		{
			Database.Database.Instance.Connect();
			List<SymbolModel> uprawnienia = new List<SymbolModel>()
			{
				new SymbolModel(){Kolor = Color.FromArgb(255, 69,169,255)}, //6
				new SymbolModel(){Kolor = Color.FromArgb(255, 255, 0, 0)}, //2
				new SymbolModel(){Kolor = Color.FromArgb(255, 255, 60, 255)}, //3
				new SymbolModel(){Kolor = Color.FromArgb(255, 0, 0, 255)}, //4
				new SymbolModel(){Kolor = Color.FromArgb(255, 0, 255,0)}, //5
				new SymbolModel(){Kolor = Color.FromArgb(255, 255,255,0)}, //6
			};

			uprawnienia.ForEach(uprawnienie => Database.Database.Instance.Insert(uprawnienie));
			Database.Database.Instance.Dispose();
		}

		private void InsertKategorie()
		{
			Database.Database.Instance.Connect();
			List<KategoriaModel> uprawnienia = new List<KategoriaModel>()
			{
				new KategoriaModel(){IdS = 3, Nazwa = "Mieszkanie", Rodzaj = -1}, //1
				new KategoriaModel(){IdS = 5, Nazwa = "Wypłata", Rodzaj = 1}, // 2
				new KategoriaModel(){IdS = 5, Nazwa = "Premia", Rodzaj = 1}, //3
				new KategoriaModel(){IdS = 4, Nazwa = "Jedzenie", Rodzaj = -1}, //4
				new KategoriaModel(){IdS = 4, Nazwa = "Picie", Rodzaj = -1, IdKRodzic = 4}, //5
				new KategoriaModel(){IdS = 4, Nazwa = "Śniadania", Rodzaj = -1, IdKRodzic = 4}, //6
				new KategoriaModel(){IdS = 4, Nazwa = "Obiady", Rodzaj = -1, IdKRodzic = 4}, //7
				new KategoriaModel(){IdS = 4, Nazwa = "Kolacje", Rodzaj = -1, IdKRodzic = 4}, //8
				new KategoriaModel(){IdS = 4, Nazwa = "Słodycze", Rodzaj = -1, IdKRodzic = 4}, //9
				new KategoriaModel(){IdS = 6, Nazwa = "Transport", Rodzaj = -1}, //10
				new KategoriaModel(){IdS = 6, Nazwa = "Bilety", Rodzaj = -1, IdKRodzic = 10}, //11
				new KategoriaModel(){IdS = 6, Nazwa = "Paliwo", Rodzaj = -1, IdKRodzic = 10}, //12
				new KategoriaModel(){IdS = 1, Nazwa = "Ogólne", Rodzaj = -1}, //113
			};

			uprawnienia.ForEach(uprawnienie => Database.Database.Instance.Insert(uprawnienie));
			Database.Database.Instance.Dispose();
		}

		private void InsertTransakcje()
		{
			Database.Database.Instance.Connect();
			List<TransakcjaModel> uprawnienia = new List<TransakcjaModel>()
			{
				new TransakcjaModel(){Data = DateTime.Now, IdO = 1, IdK = 2, Kwota = 4500, Tytul = "Wypłata"},
				new TransakcjaModel(){Data = DateTime.Now, IdO = 1, IdK = 3, Kwota = 500.50, Tytul = "Premia kwaetalna", Opis = "Premia kwartalna za świetne wyniki w firmie"},
				new TransakcjaModel(){Data = DateTime.Now, IdO = 2, IdK = 2, Kwota = 3780.24, Tytul = "Biuro finansowe - wypłata"},
				new TransakcjaModel(){Data = DateTime.Now, IdO = 1, IdK = 1, Kwota = 3300, Tytul = "Opłata za mieszkanie"},
				new TransakcjaModel(){Data = DateTime.Now, IdO = 2, IdK = 7, Kwota = 336.69, Tytul = "Mięso i warzywka"},
				new TransakcjaModel(){Data = DateTime.Now, IdO = 3, IdK = 7, Kwota = 30, Tytul = "Pizza z kolegami"},
				new TransakcjaModel(){Data = DateTime.Now, IdO = 3, IdK = 5, Kwota = 62.80, Tytul = "Piwo"},
				new TransakcjaModel(){Data = DateTime.Now, IdO = 4, IdK = 11, Kwota = 160, Tytul = "Bilet semestralny"},
				new TransakcjaModel(){Data = DateTime.Now, IdO = 5, IdK = 9, Kwota = 12.50, Tytul = "Ciasteczka"}
			};

			uprawnienia.ForEach(uprawnienie => Database.Database.Instance.Insert(uprawnienie));
			Database.Database.Instance.Dispose();
		}
	}
}
