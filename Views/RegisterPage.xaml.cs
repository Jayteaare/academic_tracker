using Microsoft.Maui.Controls;
using System.Threading.Tasks;
using C868_WGU_Tallis_Jordan.Models;
using C868_WGU_Tallis_Jordan.Services;
using BCrypt.Net;

namespace C868_WGU_Tallis_Jordan.Views
{
    public partial class RegisterPage : ContentPage
    {
        public RegisterPage()
        {
            InitializeComponent();
        }

        private async void OnRegisterClicked(object sender, EventArgs e)
        {
            string username = UsernameEntry.Text?.Trim();
            string password = PasswordEntry.Text?.Trim();
            string confirmPassword = ConfirmPasswordEntry.Text?.Trim();

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(confirmPassword))
            {
                await DisplayAlert("Error", "All fields are required.", "OK");
                return;
            }

            if (password != confirmPassword)
            {
                await DisplayAlert("Error", "Passwords do not match.", "OK");
                return;
            }

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

            var newUser = new User
            {
                Username = username,
                HashedPassword = hashedPassword
            };

            await DatabaseService.AddEntity(newUser);

            await DisplayAlert("Success", "Account registered successfully.", "OK");

            Application.Current.MainPage = new NavigationPage(new LoginPage());
        }
    }
}