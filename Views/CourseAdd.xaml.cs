using C868_WGU_Tallis_Jordan.Models;
using C868_WGU_Tallis_Jordan.Services;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace C868_WGU_Tallis_Jordan.Views
{
    public partial class CourseAdd : ContentPage
    {
        private readonly int _selectedTermId;

        public CourseAdd(int selectedTermId)
        {
            InitializeComponent();
            _selectedTermId = selectedTermId;
        }

        private async void CancelCourse_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

        private async void SaveCourse_Clicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(CourseName.Text))
            {
                await DisplayAlert("Missing Name", "Please enter a name for the course.", "Ok");
                return;
            }

            if (CourseStatusPicker.SelectedIndex == -1)
            {
                await DisplayAlert("Missing Status", "Please select a status for the course.", "Ok");
                return;
            }

            if (string.IsNullOrWhiteSpace(InstructorName.Text))
            {
                await DisplayAlert("Missing Instructor Name", "Please enter an instructor name for the course.", "Ok");
                return;
            }

            if (string.IsNullOrWhiteSpace(InstructorPhone.Text))
            {
                await DisplayAlert("Missing Instructor Phone", "Please enter an instructor phone number for the course.", "Ok");
                return;
            }

            var phoneRegex = new Regex(@"^(\(\d{3}\)\s|\d{3}-)\d{3}-\d{4}$");
            if (!phoneRegex.IsMatch(InstructorPhone.Text))
            {
                await DisplayAlert("Invalid Phone Format", "Please enter a valid phone number (e.g., (555) 555-5555 or 555-555-5555).", "Ok");
                return;
            }

            if (string.IsNullOrWhiteSpace(InstructorEmail.Text))
            {
                await DisplayAlert("Missing Instructor Email", "Please enter an instructor email for the course.", "Ok");
                return;
            }

            var emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
            if (!emailRegex.IsMatch(InstructorEmail.Text))
            {
                await DisplayAlert("Invalid Email Format", "Please enter a valid email address (e.g., user@company.com).", "Ok");
                return;
            }

            if (EndDatePicker.Date < StartDatePicker.Date)
            {
                await DisplayAlert("Invalid Date Range", "The end date cannot be before the start date.", "Ok");
                return;
            }

            var term = await DatabaseService.GetEntityById<Term>(_selectedTermId);
            if (term == null)
            {
                await DisplayAlert("Error", "Unable to retrieve term details.", "Ok");
                return;
            }

            if (StartDatePicker.Date < term.StartDate || EndDatePicker.Date > term.EndDate)
            {
                await DisplayAlert("Invalid Course Dates", "The course dates must fall within the term's start and end dates.", "Ok");
                return;
            }

            var newCourse = new Course
            {
                TermId = _selectedTermId,
                CourseName = CourseName.Text,
                StartDate = StartDatePicker.Date,
                EndDate = EndDatePicker.Date,
                Status = CourseStatusPicker.SelectedItem?.ToString() ?? string.Empty,
                InstructorName = InstructorName.Text,
                InstructorPhone = InstructorPhone.Text,
                InstructorEmail = InstructorEmail.Text,
                Notes = Notes.Text,
                Notifications = Notifications.IsToggled
            };

            await DatabaseService.AddEntity(newCourse);
            await Navigation.PopAsync();
        }
    }
}