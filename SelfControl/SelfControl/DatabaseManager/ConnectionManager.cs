using SelfControl.Interfaces;
using SelfControl.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using SQLite;
using SelfControl.Models;
using System.Threading.Tasks;

namespace SelfControl.DatabaseManager
{
    class ConnectionManager
    {
        readonly SQLiteAsyncConnection database;

        public ConnectionManager(string filePath)
        {
            database = new SQLiteAsyncConnection(filePath);
            database.CreateTableAsync<FoodItem>().Wait();
        }

        public Task<int> SaveItemAsync(FoodItem item)
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

        public Task<int> DeleteItemAsync(FoodItem item)
        {
            return database.DeleteAsync(item);
        }

        async public void printItems()
        {
            var table = database.Table<FoodItem>();
            List<FoodItem> l = await table.ToListAsync();
            foreach (var s in l)
            {
                Console.WriteLine("Food Item: " + s.ID + " " + s.NAME);
            }
        }

        async public Task<List<FoodItem>> QueryByDateTime()
        {
            var query = await database.QueryAsync<FoodItem>(DatabaseQuery.orderByDateTime, FoodItem.dateTimeCol);
            return query;
        }
    }
}
