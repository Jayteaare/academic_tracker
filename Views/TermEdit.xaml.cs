using C868_WGU_Tallis_Jordan.Models;
using C868_WGU_Tallis_Jordan.Services;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace C868_WGU_Tallis_Jordan.Views
{
    public partial class TermEdit : ContentPage
    {
        private readonly int _selectedTermId;

        public TermEdit(Term term)
        {
            InitializeComponent();

            _selectedTermId = term.Id;
            TermName.Text = term.TermName;
            StartDatePicker.Date = term.StartDate;
            EndDatePicker.Date = term.EndDate;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadCoursesForTerm();
        }

        private async Task LoadCoursesForTerm()
        {
            CourseCollectionView.ItemsSource = await DatabaseService.GetCourses(_selectedTermId);
        }

        private async void SaveTerm_OnClicked(object sender, EventArgs e)
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
                Id = _selectedTermId,
                TermName = TermName.Text,
                StartDate = StartDatePicker.Date,
                EndDate = EndDatePicker.Date
            };

            await DatabaseService.UpdateEntity(term);
            await Navigation.PopAsync();
        }

        private async void CancelTerm_OnClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

        private async void AddCourse_OnClicked(object sender, EventArgs e)
        {
            int countCourses = await DatabaseService.GetCourseCountAsync(_selectedTermId);

            if (countCourses >= 6)
            {
                await DisplayAlert("Course Limit Reached", "A term can only have a maximum of 6 courses.", "OK");
                return;
            }

            await Navigation.PushAsync(new CourseAdd(_selectedTermId));
            await LoadCoursesForTerm();
        }

        private async void CourseCollectionView_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.FirstOrDefault() is Course selectedCourse)
            {
                await Navigation.PushAsync(new ViewCourse(selectedCourse));
                CourseCollectionView.SelectedItem = null;
            }
        }
    }
}