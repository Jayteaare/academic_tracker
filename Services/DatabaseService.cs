using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using SQLite;
using C868_WGU_Tallis_Jordan.Models;

namespace C868_WGU_Tallis_Jordan.Services
{
    public static class DatabaseService
    {
        private static SQLiteAsyncConnection _db;

        static async Task Init()
        {
            if (_db != null) return;

            var databasePath = Path.Combine(FileSystem.AppDataDirectory, "C868_TJ.db3");
            _db = new SQLiteAsyncConnection(databasePath);

            await _db.CreateTableAsync<User>();
            await _db.CreateTableAsync<Term>();
            await _db.CreateTableAsync<Course>();
            await _db.CreateTableAsync<Assessment>();
        }

        public static async Task<User> GetUserByUsername(string username)
        {
            await Init();
            return await _db.Table<User>().FirstOrDefaultAsync(u => u.Username == username);
        }

        public static async Task AddUser(User user)
        {
            await Init();
            await _db.InsertAsync(user);
        }

        #region Generic CRUD Methods

        public static async Task AddEntity<T>(T entity) where T : BaseEntity
        {
            await Init();
            await _db.InsertAsync(entity);
        }

        public static async Task UpdateEntity<T>(T entity) where T : BaseEntity
        {
            await Init();
            await _db.UpdateAsync(entity);
        }

        public static async Task RemoveEntity<T>(int id) where T : BaseEntity, new()
        {
            await Init();

            if (typeof(T) == typeof(Term))
            {
                var courses = await GetCourses(id);
                foreach (var course in courses)
                {
                    await RemoveEntity<Assessment>(course.Id);
                    await RemoveEntity<Course>(course.Id);
                }
            }
            else if (typeof(T) == typeof(Course))
            {
                var assessments = await GetAssessments(id);
                foreach (var assessment in assessments)
                {
                    await RemoveEntity<Assessment>(assessment.Id);
                }
            }

            await _db.DeleteAsync<T>(id);
        }

        public static async Task<T> GetEntityById<T>(int id) where T : BaseEntity, new()
        {
            await Init();
            return await _db.Table<T>().FirstOrDefaultAsync(e => e.Id == id);
        }

        public static async Task<IEnumerable<T>> GetAllEntities<T>() where T : BaseEntity, new()
        {
            await Init();
            return await _db.Table<T>().ToListAsync();
        }

        #endregion

        #region Term-Specific Methods

        public static async Task<IEnumerable<Course>> GetCourses(int termId)
        {
            await Init();
            return await _db.Table<Course>().Where(c => c.TermId == termId).ToListAsync();
        }

        public static async Task<IEnumerable<Assessment>> GetAssessments(int courseId)
        {
            await Init();
            return await _db.Table<Assessment>().Where(a => a.CourseId == courseId).ToListAsync();
        }

        #endregion

        #region Sample Data Loading

        /*
        public static async Task LoadSampleData()
        {
            await Init();

            var term = new Term
            {
                TermName = "Term 1 - Winter",
                StartDate = new DateTime(2024, 6, 30),
                EndDate = new DateTime(2024, 12, 31)
            };
            await AddEntity(term);

            var course = new Course
            {
                TermId = term.Id,
                CourseName = "Data Management - Applications",
                StartDate = new DateTime(2024, 10, 31),
                EndDate = new DateTime(2024, 12, 31),
                Status = "In-progress",
                InstructorName = "Anika Patel",
                InstructorPhone = "555-123-4567",
                InstructorEmail = "anika.patel@strimeuniversity.edu",
                Notes = "Notes",
                Notifications = true
            };
            await AddEntity(course);

            var assessments = new List<Assessment>
            {
                new Assessment
                {
                    CourseId = course.Id,
                    AssessName = "Data Management Test",
                    Type = "Objective",
                    StartDate = new DateTime(2024, 10, 31),
                    EndDate = new DateTime(2024, 12, 31),
                    Notifications = true
                },
                new Assessment
                {
                    CourseId = course.Id,
                    AssessName = "Data Management Project",
                    Type = "Performance",
                    StartDate = new DateTime(2024, 10, 31),
                    EndDate = new DateTime(2024, 12, 31),
                    Notifications = true
                }
            };

            foreach (var assessment in assessments)
            {
                await AddEntity(assessment);
            }
        }
        */

        #endregion

        #region Count Determinations

        public static async Task<int> GetAssessmentCountAsync(int courseId)
        {
            await Init();
            return await _db.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM Assessment WHERE CourseId = ?", courseId);
        }

        public static async Task<int> GetCourseCountAsync(int selectedTermId)
        {
            await Init();
            return await _db.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM Course WHERE TermId = ?", selectedTermId);
        }

        #endregion
    }
}