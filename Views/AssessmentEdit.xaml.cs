using C868_WGU_Tallis_Jordan.Models;
using C868_WGU_Tallis_Jordan.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace C868_WGU_Tallis_Jordan.Views
{
    public partial class AssessmentEdit : ContentPage
    {
        private Assessment _assessment;

        public AssessmentEdit(Assessment assessment)
        {
            InitializeComponent();
            _assessment = assessment;
            BindingContext = _assessment;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadAssessmentData();
        }

        private async Task LoadAssessmentData()
        {
            _assessment = await DatabaseService.GetEntityById<Assessment>(_assessment.Id);
            BindingContext = _assessment;

            AssessmentTypePicker.SelectedItem = _assessment.Type;
            StartDatePicker.Date = _assessment.StartDate;
            EndDatePicker.Date = _assessment.EndDate;
            Notifications.IsToggled = _assessment.Notifications;
        }

        private async void SaveAssessment_Clicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(AssessmentName.Text))
            {
                await DisplayAlert("Missing Name", "Please enter a name for the assessment.", "Ok");
                return;
            }

            if (AssessmentTypePicker.SelectedIndex == -1)
            {
                await DisplayAlert("Missing Type", "Please select a type for the assessment.", "Ok");
                return;
            }

            if (EndDatePicker.Date < StartDatePicker.Date)
            {
                await DisplayAlert("Invalid Date Range", "The end date cannot be before the start date.", "Ok");
                return;
            }

            string selectedType = AssessmentTypePicker.SelectedItem?.ToString() ?? string.Empty;

            var course = await DatabaseService.GetEntityById<Course>(_assessment.CourseId);
            if (course == null)
            {
                await DisplayAlert("Error", "Unable to retrieve course details.", "Ok");
                return;
            }

            if (StartDatePicker.Date < course.StartDate || EndDatePicker.Date > course.EndDate)
            {
                await DisplayAlert("Invalid Assessment Dates", "The assessment dates must fall within the course's start and end dates.", "Ok");
                return;
            }

            var existingAssessments = (await DatabaseService.GetAllEntities<Assessment>())
                                      .Where(a => a.CourseId == _assessment.CourseId && a.Type == selectedType && a.Id != _assessment.Id);
            bool typeExists = existingAssessments.Any();

            if (typeExists)
            {
                await DisplayAlert("Duplicate Assessment Type", $"A {selectedType} assessment already exists for this course.", "Ok");
                return;
            }

            _assessment.AssessName = AssessmentName.Text;
            _assessment.StartDate = StartDatePicker.Date;
            _assessment.EndDate = EndDatePicker.Date;
            _assessment.Type = selectedType;
            _assessment.Notifications = Notifications.IsToggled;

            await DatabaseService.UpdateEntity(_assessment);
            await Navigation.PopAsync();
        }

        private async void CancelAssessment_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}