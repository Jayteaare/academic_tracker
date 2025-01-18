using C868_WGU_Tallis_Jordan.Models;
using C868_WGU_Tallis_Jordan.Services;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace C868_WGU_Tallis_Jordan.Views
{
    public partial class CourseEdit : ContentPage
    {
        private readonly int _selectedCourseId;
        private readonly int _termId;

        public CourseEdit(Course selectedCourse)
        {
            InitializeComponent();

            _selectedCourseId = selectedCourse.Id;
            _termId = selectedCourse.TermId;

            CourseName.Text = selectedCourse.CourseName;
            StartDatePicker.Date = selectedCourse.StartDate;
            EndDatePicker.Date = selectedCourse.EndDate;
            CourseStatusPicker.SelectedItem = selectedCourse.Status;
            InstructorName.Text = selectedCourse.InstructorName;
            InstructorPhone.Text = selectedCourse.InstructorPhone;
            InstructorEmail.Text = selectedCourse.InstructorEmail;
            Notes.Text = selectedCourse.Notes;
            Notifications.IsToggled = selectedCourse.Notifications;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadAssessments();
        }

        private async Task LoadAssessments()
        {
            var assessments = await DatabaseService.GetAssessments(_selectedCourseId);
            AssessmentCollectionView.ItemsSource = assessments;
        }

        private async void AddAssessment_Clicked(object sender, EventArgs e)
        {
            int assessmentCount = await DatabaseService.GetAssessmentCountAsync(_selectedCourseId);

            if (assessmentCount >= 2)
            {
                await DisplayAlert("Limit Reached", "A course can only have a maximum of 2 assessments.", "OK");
                return;
            }

            await Navigation.PushAsync(new AssessmentAdd(_selectedCourseId));
            await LoadAssessments();
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

            var term = await DatabaseService.GetEntityById<Term>(_termId);
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

            string status = CourseStatusPicker.SelectedItem?.ToString() ?? "Not Started";

            var updatedCourse = new Course
            {
                Id = _selectedCourseId,
                TermId = _termId,
                CourseName = CourseName.Text,
                StartDate = StartDatePicker.Date,
                EndDate = EndDatePicker.Date,
                Status = status,
                InstructorName = InstructorName.Text,
                InstructorPhone = InstructorPhone.Text,
                InstructorEmail = InstructorEmail.Text,
                Notes = Notes.Text,
                Notifications = Notifications.IsToggled
            };

            await DatabaseService.UpdateEntity(updatedCourse);

            await Navigation.PopAsync();
        }

        private async void CancelCourse_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

        public async void OnAssessmentSelected(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.FirstOrDefault() is Assessment selectedAssessment)
            {
                await Navigation.PushAsync(new ViewAssessment(selectedAssessment));
                AssessmentCollectionView.SelectedItem = null;
            }
        }
    }
}