using C868_WGU_Tallis_Jordan.Models;
using C868_WGU_Tallis_Jordan.Services;
using Plugin.LocalNotification;
using Plugin.LocalNotification.AndroidOption;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace C868_WGU_Tallis_Jordan.Views
{
    public partial class TermList : ContentPage
    {
        private bool _notificationsShown;

        public ObservableCollection<Term> Terms { get; set; } = new ObservableCollection<Term>();

        public TermList()
        {
            InitializeComponent();
            BindingContext = this;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            if (Services.Settings.FirstRun)
            {
                //await DatabaseService.LoadSampleData();
                Services.Settings.FirstRun = false;
            }

            await RefreshTermCollectionView();

            if (!_notificationsShown)
            {
                ShowBundledNotifications();
                _notificationsShown = true;
            }
        }

        private async Task RefreshTermCollectionView()
        {
            Terms.Clear();
            var terms = await DatabaseService.GetAllEntities<Term>();
            foreach (var term in terms)
            {
                Terms.Add(term);
            }
        }

        private async void ShowBundledNotifications()
        {
            if (!await LocalNotificationCenter.Current.AreNotificationsEnabled())
            {
                await LocalNotificationCenter.Current.RequestNotificationPermission();
            }

            var courseList = await DatabaseService.GetAllEntities<Course>();
            var notificationDetails = new List<string>();

            foreach (var course in courseList)
            {
                if (course.Notifications)
                {
                    if (course.StartDate == DateTime.Today)
                    {
                        notificationDetails.Add($"{course.CourseName} starts today!");
                    }

                    if (course.EndDate == DateTime.Today)
                    {
                        notificationDetails.Add($"{course.CourseName} ends today!");
                    }
                }

                var assessments = await DatabaseService.GetAssessments(course.Id);
                foreach (var assessment in assessments.Where(a => a.Notifications))
                {
                    if (assessment.StartDate == DateTime.Today)
                    {
                        notificationDetails.Add($"{assessment.AssessName} starts today!");
                    }

                    if (assessment.EndDate == DateTime.Today)
                    {
                        notificationDetails.Add($"{assessment.AssessName} ends today!");
                    }
                }
            }

            if (notificationDetails.Any())
            {
                var notificationContent = string.Join("\n", notificationDetails);
                var bundledNotification = new NotificationRequest
                {
                    Title = "Today's Academic Events",
                    Description = notificationContent,
                    BadgeNumber = 42,
                    Schedule = new NotificationRequestSchedule
                    {
                        NotifyTime = DateTime.Now.AddSeconds(5)
                    },
                    Android = new AndroidOptions
                    {
                        ChannelId = "bundled_events_channel"
                    }
                };

                await LocalNotificationCenter.Current.Show(bundledNotification);
            }
        }

        private async void AddTerm_OnClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new TermAdd());
        }

        private async void ReportsButton_OnClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ViewReports());
        }

        private async void OnTermSelected(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.FirstOrDefault() is Term selectedTerm)
            {
                await Navigation.PushAsync(new ViewTerm(selectedTerm));
                TermCollectionView.SelectedItem = null;
            }
        }

        private async void OnSearchButtonClicked(object sender, EventArgs e)
        {
            string query = SearchInput.Text?.Trim();

            if (string.IsNullOrWhiteSpace(query))
            {
                await DisplayAlert("Invalid Search", "Please enter a search query.", "OK");
                return;
            }

            await Navigation.PushAsync(new ViewSearch(query));
        }
    }
}