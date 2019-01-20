using ISBD.Model;
using ISBD.ModelView;
using ISBD.ModelView.State.Connectors;
using ISBD.ModelView.State.LogicStates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ISBD.View.Pages
{
    /// <summary>
    /// Logika interakcji dla klasy RegisterPage.xaml
    /// </summary>
    public partial class RegisterPage : Page, IRegisterUI
    {
        private string[] ErrorMessages = {
            "Ten login jest już zajęty , wybierz inny" , "Login nie może być pusty" ,
            "Podane hasła się nie zgadzają" , "Hasło nie może być puste" ,
            "Imię nie może być puste" , "Imię musi składać się jedynie z liter" ,
            "Nazwisko nie może być puste" , "Nazwisko musi składać się jedynie z liter i spacji" };

        private static string badImagePath = "/ISBD;component/Assets/Logo/error.png";
        private static string goodImagePath = "/ISBD;component/Assets/Logo/good.png";
        private BitmapImage goodImage = GetImage(new Uri(goodImagePath, UriKind.Relative));
        private BitmapImage badImage = GetImage(new Uri(badImagePath, UriKind.Relative));

        private static BitmapImage GetImage(Uri imageUri)
        {
            var bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.UriSource = imageUri;
            bitmapImage.EndInit();
            return bitmapImage;
        }
        System.Collections.Generic.List<OsobaModel> users;
        public RegisterPage()
        {
            InitializeComponent();
            Database.Database.Instance.Connect();
            users = Database.Database.Instance.SelectAll<OsobaModel>();
        }
        public string Login => LoginBox.Text;

        public string Password => PasswordBox.Password;

        public string RepeatPassword => RepeatPasswordBox.Password;

        public string UName => NameBox.Name;

        public string Surname => SurnameBox.Text;

        public Button Next => ButtonNext;

        public Button Back => ButtonBack;

        bool _loginGood = false;
        bool _passwordGood = false;
        bool _nameGood = false;
        bool _surnameGood = false;
        public void _Login(bool imageType, string message)
        {
            _loginGood = imageType;
            if (imageType)
            {
                LoginErrorText.Visibility = Visibility.Collapsed;
                LoginError.Source = goodImage;
            }
            else
            {
                LoginErrorText.Visibility = Visibility.Visible;
                LoginErrorText.Content = message;
                LoginError.Source = badImage;
            }
        }

        public void _Password(bool imageType, string message)
        {
            _passwordGood = imageType;
            if (imageType)
            {
                PasswordErrorText.Visibility = Visibility.Collapsed;
                PasswordError.Source = goodImage;
            }
            else
            {
                PasswordErrorText.Visibility = Visibility.Visible;
                PasswordErrorText.Content = message;
                PasswordError.Source = badImage;
            }
        }

        public void _Surname(bool imageType, string message)
        {
            _surnameGood = imageType;
            if (imageType)
            {
                SurnameErrorText.Visibility = Visibility.Collapsed;
                SurnameError.Source = goodImage;
            }
            else
            {
                SurnameErrorText.Visibility = Visibility.Visible;
                SurnameErrorText.Content = message;
                SurnameError.Source = badImage;
            }
        }

        public void _UName(bool imageType, string message)
        {
            _nameGood = imageType;
            if (imageType)
            {
                NameErrorText.Visibility = Visibility.Collapsed;
                NameError.Source = goodImage;
            }
            else
            {
                NameErrorText.Visibility = Visibility.Visible;
                NameErrorText.Content = message;
                NameError.Source = badImage;
            }
        }

        private void LoginBox_LostFocus(object sender, RoutedEventArgs e)
        {
            bool found = false;
            if (Login == "")
            {
                _Login(false, ErrorMessages[1]);
                found = true;
            }
            else
            {
                foreach (var Osoba in users)//tu mogłoby być lepiej
                    if (Osoba.Login == Login)
                    {
                        _Login(false, ErrorMessages[0]);
                        found = true;
                    }
            }
            if(!found)
            {
                _Login(true, null);
            }
        }

        private void NameBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Regex.IsMatch(NameBox.Text, @"^[\\s AaĄąBbCcĆćDdEeĘęFfGgHhIiJjKkLlŁłMmNnŃńOoÓóPpRrSsŚśTtUuWwYyZzŹźŻż]+$") || NameBox.Text == "")
            {
                _UName(true, null);
            }
            else
            {
                _UName(false, ErrorMessages[5]);
            }   

        }

        private void PasswordBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (Password == "")
            {
                _Password(false, ErrorMessages[3]);
            }
            else if (RepeatPassword != "" && Password != RepeatPassword)
            {
                _Password(false, ErrorMessages[2]);
            }
            else _Password(true, null);
        }

        private void NameBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (NameBox.Text == "")
            {
                _UName(false, ErrorMessages[4]);
            }
        }
        private void SurnameBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (Surname == "")
            {
                _Surname(false, ErrorMessages[6]);
            }
        }

        private void LoginBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Login == "")
                _Login(false, ErrorMessages[1]);
            else _Login(true, null);
            
        }

        private void RepeatPasswordBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (Password != RepeatPassword )
            {
                _Password(false, ErrorMessages[2]);
            }
            else if(Password!=null)_Password(true, null);
        }

        private void SurnameBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Regex.IsMatch(SurnameBox.Text, @"^[\\s vVqQAaĄąBbCcĆćDdEeĘęFfGgHhIiJjKkLlŁłMmNnŃńOoÓóPpRrSsŚśTtUuWwYyZzŹźŻż]+$") || SurnameBox.Text == "")
            {
                _Surname(true, null);
            }
            else
            {
                _Surname(false, ErrorMessages[7]);
            }
        }

        private void ButtonNext_Click(object sender, RoutedEventArgs e)
        {
            if(_loginGood && _passwordGood && _surnameGood && _nameGood && Password == RepeatPassword)
            {
                AdminLogin.Visibility = Visibility.Visible;
                RegisterBlock.IsEnabled = false;
            }
        }

        private void ButtonBack1_Click(object sender, RoutedEventArgs e)
        {
            LoginBox1.Text = null;
            PasswordBox1.Password = null;
            AdminLogin.Visibility = Visibility.Hidden;
            RegisterBlock.IsEnabled = true;
        }

        private void ButtonNext1_Click(object sender, RoutedEventArgs e)
        {
            Database.Database.Instance.Connect();

            var users = Database.Database.Instance.SelectAll<OsobaModel>();
            var currentLogin =
                users.FirstOrDefault(user => user.Login == LoginBox1.Text && user.Haslo == PasswordBox1.Password);
            if (currentLogin != null)
            {

                var permissions = Database.Database.Instance.SelectAll<UprawnienieModel>();
                var userPermission = permissions.FirstOrDefault(perm => perm.IdOD == currentLogin.IdO && perm.Poziom == 0);
                if(userPermission != null)
                {
                    OsobaModel newUser = new OsobaModel
                    {
                        Login = Login,
                        Haslo = Password,
                        Imie = NameBox.Text,
                        Nazwisko = Surname,
                        DataU = DateTime.Now
                    };
                    Database.Database.Instance.Insert(newUser);
                    var dbUser = Database.Database.Instance.SelectAll<OsobaModel>().FirstOrDefault(user => user.Login == newUser.Login);
                    UprawnienieModel uprawnienie1 = new UprawnienieModel { IdOBene = dbUser.IdO, IdOD = dbUser.IdO, Poziom = 1 };
                    UprawnienieModel uprawnienie2 = new UprawnienieModel { IdOBene = dbUser.IdO, IdOD = dbUser.IdO , Poziom = 2};
                    UprawnienieModel uprawnienie3 = new UprawnienieModel { IdOBene = dbUser.IdO, IdOD = dbUser.IdO, Poziom = 3 };
                    Database.Database.Instance.Insert(uprawnienie1);
                    Database.Database.Instance.Insert(uprawnienie2);
                    Database.Database.Instance.Insert(uprawnienie3);
                    Database.Database.Instance.Dispose();

                    StateMachine.Instance.PushState<LoggedinLogicState>(new LoggedinStatePushParameters()
                    {
                        user = dbUser,
                        saveLogged = false
                    });

                }
            }

            else
            {
                SurnameErrorText1.Visibility = Visibility.Visible;
            }
            Database.Database.Instance.Dispose();
        }
    }
}
