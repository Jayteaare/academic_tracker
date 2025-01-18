using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using C868_WGU_Tallis_Jordan.Models;
using C868_WGU_Tallis_Jordan.Services;

namespace C868_WGU_Tallis_Jordan.Views
{
    public partial class ViewSearch : ContentPage
    {
        private ObservableCollection<SearchResult> _searchResults = new ObservableCollection<SearchResult>();
        public ObservableCollection<SearchResult> SearchResults
        {
            get => _searchResults;
            set
            {
                _searchResults = value;
                OnPropertyChanged(nameof(SearchResults));
            }
        }

        private void OnItemTapped(object sender, EventArgs e)
        {
            Console.WriteLine("Item tapped.");
        }

        public ViewSearch(string query)
        {
            InitializeComponent();
            Console.WriteLine("ViewSearch constructor called.");
            Console.WriteLine($"SearchResultsCollectionView: {SearchResultsCollectionView != null}");
            BindingContext = this;
            PerformSearch(query);
        }

        private async void PerformSearch(string query)
        {
            Console.WriteLine("PerformSearch called.");
            SearchResults.Clear();

            query = query.ToLower();

            var terms = await DatabaseService.GetAllEntities<Term>();
            foreach (var term in terms.Where(t => t.TermName.ToLower().Contains(query)))
            {
                Console.WriteLine($"Found Term: {term.TermName}");
                SearchResults.Add(new SearchResult
                {
                    Type = "Term",
                    Name = term.TermName,
                    Details = $"{term.StartDate:MM/dd/yyyy} - {term.EndDate:MM/dd/yyyy}",
                    Entity = term
                });
            }

            var courses = await DatabaseService.GetAllEntities<Course>();
            foreach (var course in courses.Where(c => c.CourseName.ToLower().Contains(query)))
            {
                var term = await DatabaseService.GetEntityById<Term>(course.TermId);
                Console.WriteLine($"Found Course: {course.CourseName}");
                SearchResults.Add(new SearchResult
                {
                    Type = "Course",
                    Name = course.CourseName,
                    Details = $"Status: {course.Status}, Term: {term?.TermName ?? "Unknown"}",
                    Entity = course
                });
            }

            var assessments = await DatabaseService.GetAllEntities<Assessment>();
            foreach (var assessment in assessments.Where(a => a.AssessName.ToLower().Contains(query)))
            {
                var course = await DatabaseService.GetEntityById<Course>(assessment.CourseId);
                var term = course != null ? await DatabaseService.GetEntityById<Term>(course.TermId) : null;

                Console.WriteLine($"Found Assessment: {assessment.AssessName}");
                SearchResults.Add(new SearchResult
                {
                    Type = "Assessment",
                    Name = assessment.AssessName,
                    Details = $"Type: {assessment.Type}, Course: {course?.CourseName ?? "Unknown"}, Term: {term?.TermName ?? "Unknown"}",
                    Entity = assessment
                });
            }

            Console.WriteLine($"Search completed. Found {SearchResults.Count} results.");
        }

        private async void OnSearchResultTapped(object sender, TappedEventArgs e)
        {
            if (e.Parameter is SearchResult selectedResult)
            {
                Console.WriteLine($"Tapped Item Type: {selectedResult.Type}");
                Console.WriteLine($"Tapped Entity: {selectedResult.Entity?.GetType().Name}");

                switch (selectedResult.Type)
                {
                    case "Term":
                        if (selectedResult.Entity is Term term)
                        {
                            Console.WriteLine("Navigating to ViewTerm...");
                            await Navigation.PushAsync(new ViewTerm(term));
                        }
                        break;

                    case "Course":
                        if (selectedResult.Entity is Course course)
                        {
                            Console.WriteLine("Navigating to ViewCourse...");
                            await Navigation.PushAsync(new ViewCourse(course));
                        }
                        break;

                    case "Assessment":
                        if (selectedResult.Entity is Assessment assessment)
                        {
                            Console.WriteLine("Navigating to ViewAssessment...");
                            await Navigation.PushAsync(new ViewAssessment(assessment));
                        }
                        break;
                }
            }
        }

        private async void OnSearchResultSelected(object sender, SelectionChangedEventArgs e)
        {
            Console.WriteLine("OnSearchResultSelected invoked.");

            if (e.CurrentSelection.FirstOrDefault() is SearchResult selectedResult)
            {
                Console.WriteLine($"Selected Type: {selectedResult.Type}");
                Console.WriteLine($"Entity: {selectedResult.Entity?.GetType().Name}");

                switch (selectedResult.Type)
                {
                    case "Term":
                        if (selectedResult.Entity is Term term)
                        {
                            Console.WriteLine("Navigating to ViewTerm...");
                            await Navigation.PushAsync(new ViewTerm(term));
                        }
                        break;

                    case "Course":
                        if (selectedResult.Entity is Course course)
                        {
                            Console.WriteLine("Navigating to ViewCourse...");
                            await Navigation.PushAsync(new ViewCourse(course));
                        }
                        break;

                    case "Assessment":
                        if (selectedResult.Entity is Assessment assessment)
                        {
                            Console.WriteLine("Navigating to ViewAssessment...");
                            await Navigation.PushAsync(new ViewAssessment(assessment));
                        }
                        break;
                }
            }

    ((CollectionView)sender).SelectedItem = null;
        }
    }

    public class SearchResult
    {
        public string Type { get; set; }
        public string Name { get; set; }
        public string Details { get; set; }
        public object Entity { get; set; }
    }
}