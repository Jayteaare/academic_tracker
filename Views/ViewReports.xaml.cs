using System;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using C868_WGU_Tallis_Jordan.Models;
using C868_WGU_Tallis_Jordan.Services;

namespace C868_WGU_Tallis_Jordan.Views
{
    public partial class ViewReports : ContentPage
    {
        public ObservableCollection<ReportRow> ReportRows { get; private set; } = new ObservableCollection<ReportRow>();

        public ViewReports()
        {
            InitializeComponent();
            ReportCollectionView.ItemsSource = ReportRows;
        }

        private async void OnReportPickerSelectionChanged(object sender, EventArgs e)
        {
            if (ReportPicker.SelectedIndex == -1)
                return;

            string selectedReportType = ReportPicker.SelectedItem.ToString();

            switch (selectedReportType)
            {
                case "Upcoming Assessments":
                    HeaderColumn1.Text = "Assessment Name";
                    HeaderColumn2.Text = "Start Date";
                    HeaderColumn3.Text = "Type";
                    break;
                case "Course Summary":
                    HeaderColumn1.Text = "Course Name";
                    HeaderColumn2.Text = "Status";
                    HeaderColumn3.Text = "Duration";
                    break;
                case "Term Overview":
                    HeaderColumn1.Text = "Term Name";
                    HeaderColumn2.Text = "Start Date";
                    HeaderColumn3.Text = "End Date";
                    break;
                default:
                    HeaderColumn1.Text = string.Empty;
                    HeaderColumn2.Text = string.Empty;
                    HeaderColumn3.Text = string.Empty;
                    break;
            }

            await GenerateReport(selectedReportType);
        }

        private async Task GenerateReport(string reportType)
        {
            ReportRows.Clear();

            ReportTitle.Text = reportType;
            ReportTimestamp.Text = $"Generated on: {DateTime.Now:MM/dd/yyyy HH:mm:ss}";

            switch (reportType)
            {
                case "Upcoming Assessments":
                    await GenerateUpcomingAssessmentsReport();
                    break;
                case "Course Summary":
                    await GenerateCourseSummaryReport();
                    break;
                case "Term Overview":
                    await GenerateTermOverviewReport();
                    break;
            }
        }

        private async Task GenerateUpcomingAssessmentsReport()
        {
            var assessments = await DatabaseService.GetAllEntities<Assessment>();
            foreach (var assessment in assessments)
            {
                if (assessment.StartDate.Date >= DateTime.Today)
                {
                    ReportRows.Add(new ReportRow
                    {
                        Column1 = assessment.AssessName,
                        Column2 = assessment.StartDate.ToString("MM/dd/yyyy"),
                        Column3 = assessment.Type
                    });
                }
            }
        }

        private async Task GenerateCourseSummaryReport()
        {
            var courses = await DatabaseService.GetAllEntities<Course>();
            foreach (var course in courses)
            {
                ReportRows.Add(new ReportRow
                {
                    Column1 = course.CourseName,
                    Column2 = course.Status,
                    Column3 = $"{course.StartDate:MM/dd/yyyy} - {course.EndDate:MM/dd/yyyy}"
                });
            }
        }

        private async Task GenerateTermOverviewReport()
        {
            var terms = await DatabaseService.GetAllEntities<Term>();
            foreach (var term in terms)
            {
                ReportRows.Add(new ReportRow
                {
                    Column1 = term.TermName,
                    Column2 = $"{term.StartDate:MM/dd/yyyy}",
                    Column3 = $"{term.EndDate:MM/dd/yyyy}"
                });
            }
        }

        private async void OnExportReportButtonClicked(object sender, EventArgs e)
        {
            if (ReportRows.Count == 0)
            {
                await DisplayAlert("Export Error", "No report data available to export.", "OK");
                return;
            }

            var reportContent = new StringBuilder();
            reportContent.AppendLine($"{ReportTitle.Text}");
            reportContent.AppendLine($"Generated on: {DateTime.Now:MM/dd/yyyy HH:mm:ss}");
            reportContent.AppendLine();

            reportContent.AppendLine($"{HeaderColumn1.Text}, {HeaderColumn2.Text}, {HeaderColumn3.Text}");

            foreach (var row in ReportRows)
            {
                reportContent.AppendLine($"{row.Column1}, {row.Column2}, {row.Column3}");
            }

            try
            {
                string fileName = $"{ReportTitle.Text.Replace(" ", "_")}_{DateTime.Now:yyyyMMddHHmmss}.txt";

#if ANDROID
        string downloadsPath = "/storage/emulated/0/Download";
        string filePath = Path.Combine(downloadsPath, fileName);

        File.WriteAllText(filePath, reportContent.ToString());

        await DisplayAlert("Export Success", $"Report saved to: {filePath}", "OK");
#else
                string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                string filePath = Path.Combine(documentsPath, fileName);

                File.WriteAllText(filePath, reportContent.ToString());

                await DisplayAlert("Export Success", $"Report saved to: {filePath}", "OK");
#endif
            }
            catch (Exception ex)
            {
                await DisplayAlert("Export Error", $"Failed to export report: {ex.Message}", "OK");
            }
        }

        public class ReportRow
        {
            public string Column1 { get; set; }
            public string Column2 { get; set; }
            public string Column3 { get; set; }
        }
    }
}