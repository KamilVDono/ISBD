using System;
using System.Linq;
using System.Windows;
using ISBD.Model;
using ISBD.View.Pages;
using ISBD.ModelView.State.Connectors;
using ISBD.ModelView.State.LogicStates;
using ISBD.Utils;
using ISBD.View;
using System.Windows.Media.Imaging;
using System.Text.RegularExpressions;

namespace ISBD.ModelView.State.UIStates
{
    public class RegisterUIState : ConnectorState<IRegisterUI, RegisterPage>
    {
        private string[] ErrorMessages = {
            "Ten login jest już zajęty , wybierz inny" , "Login nie może być pusty" ,
            "Podane hasła się nie zgadzają" , "Hasło nie może być puste" ,
            "Imię nie może być puste" , "Imię musi składać się jedynie z liter" ,
            "Nazwisko nie może być puste" , "Nazwisko musi składać się jedynie z liter i spacji" };

        public override void StartState()
        {

            base.StartState();
     

            Connector.Back.Click -= GoBack;
            Connector.Back.Click += GoBack;
        }
        public void GoBack(Object sender, RoutedEventArgs e)
        {
            StateMachine.Instance.PopUIState();
        }

        private bool CheckLogin()
        {
            Database.Database.Instance.Connect();

            Boolean checkError = false;
            var users = Database.Database.Instance.SelectAll<OsobaModel>();
            if (Connector.Login == "")
            {
                Connector._Login(false, ErrorMessages[1]);
                return false;
            }
            else
            {
                foreach (var Osoba in users)
                    if (Osoba.Login == Connector.Login)
                    {
                        checkError = true;
                        Connector._Login(false, ErrorMessages[0]);
                        return false;
                    }
            }
            if (!checkError)
            {
                Connector._Login(true, null);
                return true;
            }
            return false;
        }
        private bool CheckPassword()
        {
            if (Connector.Password == "")
            {
                Connector._Password(false, ErrorMessages[3]);
                return false;
            }
            else if (Connector.Password != Connector.RepeatPassword)
            {
                Connector._Password(false, ErrorMessages[2]);
                return false;
            }
            else Connector._Password(true, null);
            return true;
        }
        private bool CheckName()
        {
            if (Connector.UName.Equals(""))
            {
                Connector._UName(false, ErrorMessages[4]);
                return false;
            }
            else if (Regex.IsMatch(Connector.UName, @"^[a-zA-Z]+$"))
            {
                Connector._UName(true, null);
                return true;
            }
            else
            {
                Connector._UName(false, ErrorMessages[5]);
                return false;
            }
        }
        private bool CheckSurname()
        {
            if (Connector.Surname == "")
            {
                Connector._Surname(false, ErrorMessages[6]);
                return false;
            }
            else if (Regex.IsMatch(Connector.Surname, @"^[a-zA-Z\s]+$"))
            {
                Connector._Surname(true, null);
                return true;
            }
            else Connector._Surname(false, ErrorMessages[7]);
            return false;
        }
    }

}

