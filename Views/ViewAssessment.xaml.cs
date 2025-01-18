using C868_WGU_Tallis_Jordan.Models;
using C868_WGU_Tallis_Jordan.Services;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace C868_WGU_Tallis_Jordan.Views
{
    public partial class ViewAssessment : ContentPage
    {
        private readonly int _assessmentId;

        public ViewAssessment(Assessment assessment)
        {
            InitializeComponent();
            _assessmentId = assessment.Id;
            BindingContext = assessment;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await RefreshAssessmentData();
        }

        private async Task RefreshAssessmentData()
        {
            var updatedAssessment = await DatabaseService.GetEntityById<Assessment>(_assessmentId);
            if (updatedAssessment != null)
            {
                BindingContext = updatedAssessment;
            }
        }

        private async void OnEditButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AssessmentEdit((Assessment)BindingContext));
        }

        private async void OnDeleteButtonClicked(object sender, EventArgs e)
        {
            bool confirmDelete = await DisplayAlert("Confirm Delete", "Are you sure you want to delete this assessment?", "Yes", "No");
            if (confirmDelete)
            {
                await DatabaseService.RemoveEntity<Assessment>(_assessmentId);
                await DisplayAlert("Deleted", "Assessment has been deleted.", "OK");
                await Navigation.PopAsync();
            }
        }
    }
}