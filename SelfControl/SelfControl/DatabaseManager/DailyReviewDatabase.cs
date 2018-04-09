using SelfControl.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SelfControl.DatabaseManager
{
    public class DailyReviewDatabase : ConnectionManager
    {
        public DailyReviewDatabase(string filePath) : base(filePath)
        {
            database.CreateTableAsync<DailyReviewTable>().Wait();
        }

        public Task<int> SaveItemAsync(DailyReviewTable item)
        {
            if (item.ID != 0)
            {
                return database.UpdateAsync(item);
            }
            else
            {
                return database.InsertAsync(item);
            }
        }

        public Task<int> DeleteItemAsync(DailyReviewTable item)
        {
            return database.DeleteAsync(item);
        }
    }
}
