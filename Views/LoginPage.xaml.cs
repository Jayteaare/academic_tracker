using Microsoft.Maui.Controls;
using System.Threading.Tasks;
using C868_WGU_Tallis_Jordan.Models;
using C868_WGU_Tallis_Jordan.Services;
using BCrypt.Net;

namespace C868_WGU_Tallis_Jordan.Views
{
    public partial class LoginPage : ContentPage
    {
        private bool _showRegisterButton;
        public bool ShowRegisterButton
        {
            get => _showRegisterButton;
            set
            {
                _showRegisterButton = value;
                OnPropertyChanged(nameof(ShowRegisterButton));
            }
        }
        public LoginPage()
        {
            InitializeComponent();
            BindingContext = this;

            CheckUserExists();
        }

        private async void CheckUserExists()
        {
            var users = await DatabaseService.GetAllEntities<User>();
            ShowRegisterButton = !users.Any();
        }

        private async void OnLoginClicked(object sender, EventArgs e)
        {
            string username = UsernameEntry.Text?.Trim();
            string password = PasswordEntry.Text?.Trim();

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                await DisplayAlert("Error", "Username and password are required.", "OK");
                return;
            }

            var user = await DatabaseService.GetUserByUsername(username);
            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.HashedPassword))
            {
                await DisplayAlert("Error", "Invalid username or password.", "OK");
                return;
            }

            await DisplayAlert("Success", "Login successful.", "OK");

            Preferences.Set("UserId", user.Id);
            Preferences.Set("SessionExpiration", DateTime.Now.AddDays(7).ToString());

            Application.Current.MainPage = new NavigationPage(new TermList());
        }

        private async void OnRegisterClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new RegisterPage());
        }
    }
}