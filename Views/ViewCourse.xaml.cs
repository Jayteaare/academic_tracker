using C868_WGU_Tallis_Jordan.Models;
using C868_WGU_Tallis_Jordan.Services;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace C868_WGU_Tallis_Jordan.Views
{
    public partial class ViewCourse : ContentPage
    {
        private Course _course;
        public ObservableCollection<Assessment> Assessments { get; set; } = new ObservableCollection<Assessment>();

        public ViewCourse(Course course)
        {
            InitializeComponent();
            _course = course;
            BindingContext = _course;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await RefreshCourseData();
            await LoadAssessments();
        }

        private async Task RefreshCourseData()
        {
            _course = await DatabaseService.GetEntityById<Course>(_course.Id);
            BindingContext = _course;
        }

        private async Task LoadAssessments()
        {
            Assessments.Clear();
            var assessments = await DatabaseService.GetAssessments(_course.Id);
            foreach (var assessment in assessments)
            {
                Assessments.Add(assessment);
            }
            AssessmentCollectionView.ItemsSource = Assessments;
        }

        private async void OnAssessmentSelected(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.FirstOrDefault() is Assessment selectedAssessment)
            {
                await Navigation.PushAsync(new ViewAssessment(selectedAssessment));
                AssessmentCollectionView.SelectedItem = null;
            }
        }

        private async void ShareNoteText_Clicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_course.Notes))
            {
                await DisplayAlert("No Notes Available", "There are no notes to share for this course.", "OK");
                return;
            }

            await Share.RequestAsync(new ShareTextRequest
            {
                Text = _course.Notes,
                Title = "Share Course Notes"
            });
        }

        private async void ShareNoteUri_Clicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_course.Notes))
            {
                await DisplayAlert("No Notes Available", "There are no notes to share for this course.", "OK");
                return;
            }

            await Share.RequestAsync(new ShareTextRequest
            {
                Uri = "https://www.example.com/courseinfo",
                Title = "Share Course URI"
            });
        }

        private async void EditCourseButton_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new CourseEdit(_course));
        }

        private async void DeleteCourseButton_Clicked(object sender, EventArgs e)
        {
            bool confirmDelete = await DisplayAlert("Confirm Delete", "Are you sure you want to delete this course?", "Yes", "No");
            if (confirmDelete)
            {
                await DatabaseService.RemoveEntity<Course>(_course.Id);
                await DisplayAlert("Course Deleted", "The course has been deleted successfully.", "OK");
                await Navigation.PopAsync();
            }
        }
    }
}