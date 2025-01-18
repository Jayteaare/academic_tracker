using C868_WGU_Tallis_Jordan.Views;
using C868_WGU_Tallis_Jordan.Services;

namespace C868_WGU_Tallis_Jordan
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            CheckSession();
        }

        private void CheckSession()
        {
            var userId = Preferences.Get("UserId", -1);
            var sessionExpiration = Preferences.Get("SessionExpiration", null);

            if (userId != -1 &&
                DateTime.TryParse(sessionExpiration, out DateTime expirationDate) &&
                expirationDate > DateTime.Now)
            {
                MainPage = new NavigationPage(new TermList());
            }
            else
            {
                MainPage = new NavigationPage(new LoginPage());
            }
        }
    }
}