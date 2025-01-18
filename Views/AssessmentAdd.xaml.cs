using C868_WGU_Tallis_Jordan.Services;
using System;
using Microsoft.Maui.Controls;
using C868_WGU_Tallis_Jordan.Models;

namespace C868_WGU_Tallis_Jordan.Views
{
    public partial class AssessmentAdd : ContentPage
    {
        private readonly int _selectedCourseId;

        public AssessmentAdd(int selectedCourseId)
        {
            InitializeComponent();
            _selectedCourseId = selectedCourseId;
        }

        private async void CancelAssessment_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
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

            var course = await DatabaseService.GetEntityById<Course>(_selectedCourseId);
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

            var existingAssessments = await DatabaseService.GetAllEntities<Assessment>();
            string selectedType = AssessmentTypePicker.SelectedItem?.ToString() ?? string.Empty;
            bool typeExists = existingAssessments.Any(a => a.CourseId == _selectedCourseId && a.Type == selectedType);

            if (typeExists)
            {
                await DisplayAlert("Duplicate Assessment", $"An assessment of type '{selectedType}' already exists for this course. Only one of each type is allowed.", "Ok");
                return;
            }

            var newAssessment = new Assessment
            {
                CourseId = _selectedCourseId,
                AssessName = AssessmentName.Text,
                StartDate = StartDatePicker.Date,
                EndDate = EndDatePicker.Date,
                Type = selectedType,
                Notifications = Notifications.IsToggled
            };

            await DatabaseService.AddEntity(newAssessment);
            await Navigation.PopAsync();
        }
    }
}