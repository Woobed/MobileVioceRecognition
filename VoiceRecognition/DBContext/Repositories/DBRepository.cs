using SQLite;
using VoiceRecognition.DBContext.Models;

namespace VoiceRecognition.DBContext.Repositories
{
    public class DBRepository
    {
        private SQLiteAsyncConnection _database;

        public DBRepository()
        {
            InitializeDatabase();
        }

        private async void InitializeDatabase()
        {
            if (_database != null) return;

            var databasePath = Path.Combine(Environment.GetFolderPath(
                Environment.SpecialFolder.LocalApplicationData), "voicerecords.db3");

            _database = new SQLiteAsyncConnection(databasePath);
            await _database.CreateTableAsync<RequestModel>();
        }

        public async Task<List<RequestModel>> GetAllRecordsAsync()
        {
            return await _database.Table<RequestModel>()
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        public async Task<RequestModel> GetRecordAsync(int id)
        {
            return await _database.Table<RequestModel>()
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync();
        }

        public async Task<int> SaveRecordAsync(RequestModel record)
        {
            if (record.Id != 0)
                return await _database.UpdateAsync(record);
            else
                return await _database.InsertAsync(record);
        }

        public async Task<int> DeleteRecordAsync(RequestModel record)
        {
            return await _database.DeleteAsync(record);
        }

        public async Task<List<RequestModel>> SearchRecordsAsync(string searchText)
        {
            return await _database.Table<RequestModel>()
                .Where(x => x.Text.Contains(searchText))
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<RequestModel>> GetFavoritesAsync()
        {
            return await _database.Table<RequestModel>()
                .Where(x => x.IsFavorite)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        public async Task ToggleFavoriteAsync(int recordId)
        {
            var record = await GetRecordAsync(recordId);
            if (record != null)
            {
                record.IsFavorite = !record.IsFavorite;
                await SaveRecordAsync(record);
            }
        }
    }
}

