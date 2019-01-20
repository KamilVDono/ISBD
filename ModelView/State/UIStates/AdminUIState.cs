using ISBD.Model;
using ISBD.ModelView.State.Connectors;
using ISBD.ModelView.State.LogicStates;
using ISBD.View.Pages;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace ISBD.ModelView.State.UIStates
{
    
        class AdminUIState : ConnectorState<IAdminUI, AdminPage>
        {
            public override void StartState()
            {
                base.StartState();

                Database.Database.Instance.Connect();
                var allUsers = Database.Database.Instance.SelectAll<OsobaModel>().ToList();
                var allPermissions = Database.Database.Instance.SelectAll<UprawnienieModel>().ToList();
                Database.Database.Instance.Dispose();

                _Users = allUsers;

                _Permissions = allPermissions;

                Connector.users.ItemsSource = _Users;

                Connector.users.SelectionChanged -= UpdateState;
                Connector.users.SelectionChanged += UpdateState;

                Connector.users.SelectedIndex = 0;

                Connector.buttonBack.Click -= goBack;
                Connector.buttonBack.Click += goBack;

                Connector.saveData.Click -= saveData;
                Connector.saveData.Click += saveData;

                Connector.isAdmin.Click -= checkSettable;
                Connector.isAdmin.Click += checkSettable;
        }

        private void checkSettable(object sender, RoutedEventArgs e)
        {
            if (_currentUser == 1)//żeby admina nie zabierać
                Connector.isAdmin.IsChecked=true;
        }

        private void goBack(object sender, RoutedEventArgs e)
        {
            StateMachine.Instance.PopUIState();
        }

        private bool dataChanged()
        {
            var kupa = getValues();
            _updated = false;
            for(int i = 0; i < _Users.Count() && !_updated; i++)
            {
                for(int j = 0; j < 5 && !_updated; j++)
                {
                    if (_Cells[i, j] != kupa[i, j])
                        _updated = true;
                }
            }
            return _updated;
        }
        
        private void saveData(object sender, RoutedEventArgs e)
        {
            Database.Database.Instance.Connect();
            var kupa = getValues();
            for (int i = 0; i < _Users.Count(); i++)
            {
                for (int j = 0; j < 5 && !_updated; j++)
                {
                    if (_Cells[i, j] != kupa[i, j])
                    {
                        int level = 0;
                        if (j == 3) level = -1;
                        else if (j == 4) level = -2;
                        else level = j + 1;
                        if (_Cells[i, j] == true)
                            Database.Database.Instance.Delete(_Permissions.FirstOrDefault(perm => perm.IdOBene == _currentUser && perm.IdOD == i+1 && perm.Poziom == level));
                        else 
                             {
                            UprawnienieModel model = new UprawnienieModel {
                                IdU = 0 ,
                                IdOBene = _currentUser,
                                IdOD = i + 1 ,
                                Poziom = level
                            };
                            Database.Database.Instance.Insert(model);
                              }
                    }
                }
                
            }
            if (_isAdmin != Connector.isAdmin.IsChecked)
                if ((bool)Connector.isAdmin.IsChecked)
                    Database.Database.Instance.Insert(new UprawnienieModel { IdOBene = _currentUser, IdOD = _currentUser, Poziom = 0 });
                else Database.Database.Instance.Delete(_Permissions.FirstOrDefault(perm => perm.IdOBene == _currentUser && perm.IdOD == _currentUser && perm.Poziom == 0));

            _Permissions = Database.Database.Instance.SelectAll<UprawnienieModel>();

            Database.Database.Instance.Dispose();
        }
        bool _updated = false;

        List<OsobaModel> _Users;

        List<UprawnienieModel> _Permissions;

        long _currentUser = -1;

        bool[,] _Cells;

        bool _isAdmin;

        private void UpdateState(object sender, SelectionChangedEventArgs e)
        {

            if (Connector.users.SelectedItem == null)
            { 
                Connector.users.SelectedItem = _Users.ElementAt(0);
                }

            _currentUser = ((OsobaModel)Connector.users.SelectedItem).IdO;

            var usersPermissions = _Permissions.FindAll(user => user.IdOBene == _currentUser);


            if (Connector.users.SelectedItem == null)
                 usersPermissions = _Permissions.FindAll(user => user.IdOBene == 1);


            Connector.isAdmin.IsChecked = false;
            _isAdmin = false;

            AddListElements(usersPermissions);
        }
        private List<OsobaModel> GetUsers()
        {
            Database.Database.Instance.Connect();

            var allPermissionsUsers = Database.Database.Instance.SelectAll<OsobaModel>().ToList();

            Database.Database.Instance.Dispose();

            return allPermissionsUsers;
        }

        public void AddListElements(List<UprawnienieModel> permList)
        {
            List<DataGridItem> list = new List<DataGridItem>();

            bool[,] crud = new bool[_Users.Count() , 5];

            foreach (UprawnienieModel perm in permList)
            {
                if(perm.IdOBene.Equals(_currentUser)) //wybierz tylko te które należą do wybranego użytkownika
                {

                    if (perm.Poziom > 0)//perm == 1 odczytaj || perm == 2 dodaj|| perm == 3 usuń
                        crud[perm.IdOD-1, perm.Poziom - 1] = true;

                    else if (perm.Poziom.Equals(-1))
                        crud[perm.IdOD-1, 3] = true;
                    else if (perm.Poziom.Equals(-2))
                        crud[perm.IdOD - 1, 4] = true;
                    else if (perm.IdOD == _currentUser)
                    {
                        if (perm.Poziom == 0)
                        {
                            _isAdmin = true;
                            Connector.isAdmin.IsChecked = true;
                        }
                    }

                }
            }
            _Cells = crud;
            foreach (OsobaModel user in _Users)
            {
                list.Add(new DataGridItem
                {
                    Imie = user.Imie,
                    Nazwisko = user.Nazwisko,
                    Login = user.Login,
                    Odczytaj = crud[user.IdO - 1, 0],
                    Dodaj = crud[user.IdO - 1, 1],
                    Usun = crud[user.IdO - 1, 2],
                    Zakaz = crud[user.IdO - 1, 3],
                    ZakazP = crud[user.IdO -1 ,4]
                });
            }
            Connector.data.ItemsSource = list;
            Connector.data.Items.Refresh();

        }
        public bool[,] getValues()
        {
            bool[,] arr = new bool[_Users.Count(), 5];
            int i = 0;
            var items = Connector.data.Items;
            foreach(DataGridItem item in items)
            {
                arr[i, 0] = item.Odczytaj;
                arr[i, 1] = item.Dodaj;
                arr[i, 2] = item.Usun;
                arr[i, 3] = item.Zakaz;
                arr[i, 4] = item.ZakazP;
                i++;
            }
            return arr;
        }


        public class DataGridItem
        {
            public string Imie { get; set; }
            public string Nazwisko { get; set; }
            public string Login { get; set; }
            public bool Odczytaj { get; set; }
            public bool Dodaj { get; set; }
            public bool Usun { get; set; }
            public bool Zakaz { get; set; }
            public bool ZakazP { get; set; }
        }
    }
}
