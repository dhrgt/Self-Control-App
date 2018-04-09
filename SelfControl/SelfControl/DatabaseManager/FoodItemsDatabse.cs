using SelfControl.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SelfControl.DatabaseManager
{
    public class FoodItemsDatabse : ConnectionManager
    {
        public FoodItemsDatabse(string filePath) : base(filePath)
        {
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

        async public Task<List<FoodItem>> QueryById(int id)
        {
            var query = await database.QueryAsync<FoodItem>(DatabaseQuery.getItemById, id);
            return query;
        }

        async public Task<List<FoodItem>> QueryIdByDate(DateTime dateTime)
        {
            var query = await database.QueryAsync<FoodItem>(DatabaseQuery.getIdByDate, dateTime);
            return query;
        }
    }
}
