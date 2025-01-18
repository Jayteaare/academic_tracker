using C868_WGU_Tallis_Jordan.Models;
using C868_WGU_Tallis_Jordan.Services;
using System;
using Microsoft.Maui.Controls;

namespace C868_WGU_Tallis_Jordan.Views
{
    public partial class TermAdd : ContentPage
    {
        public TermAdd()
        {
            InitializeComponent();
        }

        private async void SaveTerm_Clicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TermName.Text))
            {
                await DisplayAlert("Missing Name", "Please enter a name for the term.", "Ok");
                return;
            }

            if (EndDatePicker.Date < StartDatePicker.Date)
            {
                await DisplayAlert("Invalid Date Range", "The end date cannot be before the start date.", "Ok");
                return;
            }

            var term = new Term
            {
                TermName = TermName.Text,
                StartDate = StartDatePicker.Date,
                EndDate = EndDatePicker.Date
            };

            await DatabaseService.AddEntity(term);

            await Navigation.PopAsync();
        }

        private async void CancelTerm_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}