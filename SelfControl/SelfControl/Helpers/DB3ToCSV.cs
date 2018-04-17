using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using SelfControl.Models;
using Xamarin.Forms;

namespace SelfControl.Helpers
{
    class DB3ToCSV
    {
        List<FoodItem> foodItems;
        List<DailyReviewTable> dailyReviewItems;
        List<WeeklyReviewTable> weeklyReviewItems;
        
        public DB3ToCSV()
        {
            Task.Run(() => SaveFoodItems());
            Task.Run(() => SaveDailyItems());
            Task.Run(() => SaveWeeklyItems());
        }

        async private void SaveFoodItems()
        {
            foodItems = await GlobalVariables.foodItemsDatabse.QueryByDateTime();
            StringBuilder tableContent = new StringBuilder();

            tableContent.Append(FoodItem.idCol + "," +
                FoodItem.dateTimeCol + "," +
                FoodItem.nameCol + "," +
                FoodItem.answersCol);
            tableContent.AppendLine();

            int id = -1;
            DateTime dateTime = new DateTime();
            string name = String.Empty;
            Dictionary<int, int> answers = null;

            foreach (var food in foodItems)
            {
                id = food.ID;
                dateTime = food.DATE;
                name = food.NAME;
                answers = GlobalVariables.DeserializeDictionary(food.ANSWERS);
                
                StringBuilder ans = new StringBuilder();
                foreach(var a in answers)
                {
                    ans.Append(a.Key + " = " + a.Value);
                    ans.AppendLine();
                    ans.Append(" , , ,");
                }

                tableContent.Append(id + "," + dateTime + "," + name + "," + ans);
                tableContent.AppendLine();
            }
            
            var filePath = DependencyService.Get<Interfaces.IFileHelper>().GetExternalFilePath("FoodTable.csv");
            File.WriteAllText(filePath, tableContent.ToString());
        }

        async private void SaveDailyItems()
        {
            dailyReviewItems = await GlobalVariables.dailyReviewDatabase.QueryByDateTime();
            StringBuilder tableContent = new StringBuilder();

            tableContent.Append(DailyReviewTable.idCol + "," +
                DailyReviewTable.dateCreatedCol + "," +
                DailyReviewTable.dateTimeCol + "," +
                DailyReviewTable.dayCol + "," +
                DailyReviewTable.isCompletedCol + "," +
                DailyReviewTable.responseCol);
            tableContent.AppendLine();

            int id = -1;
            DateTime dateCreated = new DateTime();
            DateTime dateCompleted = new DateTime();
            int day = -1;
            bool isCompleted = false;
            Dictionary<int, int> responses = null;

            foreach(var item in dailyReviewItems)
            {
                id = item.ID;
                dateCreated = item.DATECREATED;
                dateCompleted = item.DATE;
                day = item.DAY;
                isCompleted = item.ISCOMPLETED;
                responses = GlobalVariables.DeserializeDictionary(item.RESPONSE);

                StringBuilder ans = new StringBuilder();
                foreach (var a in responses)
                {
                    ans.Append(a.Key + " = " + a.Value);
                    ans.AppendLine();
                    ans.Append(" , , , , ,");
                }

                tableContent.Append(id + "," + dateCreated + "," + dateCompleted + "," + day + "," + isCompleted + "," + ans);
                tableContent.AppendLine();
            }
            var filePath = DependencyService.Get<Interfaces.IFileHelper>().GetExternalFilePath("DailyReview.csv");
            File.WriteAllText(filePath, tableContent.ToString());
        }

        async private void SaveWeeklyItems()
        {
            weeklyReviewItems = await GlobalVariables.weeklyReviewDatabse.QueryByDateTime();
            StringBuilder tableContent = new StringBuilder();

            tableContent.Append(WeeklyReviewTable.idCol + "," +
                WeeklyReviewTable.dateTimeCol + "," +
                WeeklyReviewTable.weekCol + "," +
                WeeklyReviewTable.isCompletedCol + "," +
                "_pictureID" + "," +
                WeeklyReviewTable.responseCol);
            tableContent.AppendLine();

            int id = -1;
            DateTime dateCompleted = new DateTime();
            int week = -1;
            bool isCompleted = false;
            string responses = String.Empty;

            foreach (var item in weeklyReviewItems)
            {
                id = item.ID;
                dateCompleted = item.DATE;
                week = item.WEEK;
                isCompleted = item.ISCOMPLETED;
                responses = item.RESPONSE;

                tableContent.Append(id + "," + dateCompleted + "," + week + "," + isCompleted + "," + responses);
                tableContent.AppendLine();
            }
            var filePath = DependencyService.Get<Interfaces.IFileHelper>().GetExternalFilePath("WeeklyReview.csv");
            File.WriteAllText(filePath, tableContent.ToString());
        }
    }
}
