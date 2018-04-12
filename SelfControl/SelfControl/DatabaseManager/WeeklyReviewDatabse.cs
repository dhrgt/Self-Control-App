using SelfControl.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SelfControl.DatabaseManager
{
    public class WeeklyReviewDatabse : ConnectionManager
    {
        public WeeklyReviewDatabse(string filePath) : base(filePath)
        {
            database.CreateTableAsync<WeeklyReviewTable>().Wait();
        }

        public Task<int> SaveItemAsync(WeeklyReviewTable item)
        {
            return database.InsertAsync(item);
        }

        public Task<int> UpdateItemAsync(WeeklyReviewTable item)
        {
            return database.UpdateAsync(item);
        }

        public Task<int> DeleteItemAsync(WeeklyReviewTable item)
        {
            return database.DeleteAsync(item);
        }

        async public Task<List<WeeklyReviewTable>> QueryByDateTime()
        {
            var query = await database.QueryAsync<WeeklyReviewTable>("SELECT * FROM WeeklyReview ORDER BY ?", WeeklyReviewTable.dateTimeCol);
            return query;
        }
    }
}
