using C868_WGU_Tallis_Jordan.Models;
using C868_WGU_Tallis_Jordan.Services;
using System.Threading.Tasks;

namespace C868_WGU_Tallis_Jordan.Views
{
    public partial class ViewTerm : ContentPage
    {
        private Term _term;

        public ViewTerm(Term term)
        {
            InitializeComponent();
            _term = term;
            BindingContext = _term;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await RefreshTermData();
        }

        private async Task RefreshTermData()
        {
            _term = await DatabaseService.GetEntityById<Term>(_term.Id);
            BindingContext = _term;
            CourseCollectionView.ItemsSource = await DatabaseService.GetCourses(_term.Id);
        }

        private async void OnCourseSelected(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.FirstOrDefault() is Course selectedCourse)
            {
                await Navigation.PushAsync(new ViewCourse(selectedCourse));
                CourseCollectionView.SelectedItem = null;
            }
        }

        private async void EditTermButton_OnClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new TermEdit(_term));
        }

        private async void DeleteTermButton_OnClicked(object sender, EventArgs e)
        {
            bool confirmDelete = await DisplayAlert("Confirm Delete", "Are you sure you want to delete this term?", "Yes", "No");
            if (confirmDelete)
            {
                await DatabaseService.RemoveEntity<Term>(_term.Id);
                await DisplayAlert("Term Deleted", "The term has been deleted successfully.", "OK");
                await Navigation.PopAsync();
            }
        }
    }
}