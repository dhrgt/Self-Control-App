using SelfControl.DatabaseManager;
using SelfControl.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace SelfControl.Helpers
{
    public class GlobalVariables
    {
        public const int STAGE_1 = 1;
        public const int STAGE_2 = 2;

        public const int SIZE_OF_FOOD_LIBRARY = 3;

        public const double CAMERA_PREVIEW_SCALE = 1.0;

        public const double CAMERA_ROI_SIZE = 5.0;

        public const int PRACTICE_NUMBER = 3;

        public const string DATABASE_NAME = "foodDB.db3";

        public const bool SaturationAnimation = true;
        public const bool ZoomAnimation = true;

        public const float StartingZoomValue = 1.2f;

        private static Dictionary<DateTime, List<FoodItem>> dateDiary = new Dictionary<DateTime, List<FoodItem>>();

        private static List<FoodItem> FoodItems = new List<FoodItem>();

        public static FoodItemsDatabase foodItemsDatabase = new FoodItemsDatabase(DependencyService.Get<Interfaces.IFileHelper>().GetLocalFilePath(DATABASE_NAME));
        public static DailyReviewDatabase dailyReviewDatabase = new DailyReviewDatabase(DependencyService.Get<Interfaces.IFileHelper>().GetLocalFilePath(DATABASE_NAME));
        public static WeeklyReviewDatabse weeklyReviewDatabse = new WeeklyReviewDatabse(DependencyService.Get<Interfaces.IFileHelper>().GetLocalFilePath(DATABASE_NAME));

        public enum AspectRatio
        {
            FourByThree = 1,
            SixteenByNine = 2
        }

        public enum GalleryMode
        {
            Normal = 1,
            Selection = 2
        }

        public enum RandomCriteria
        {
            Random = 1,
            DeltaFrequency = 2,
            Health = 3,
            HealthAndFrequency = 4
        }

        public static Dictionary<int, string> Questions = new Dictionary<int, string>()
        {
            { 0, "How frequently do you eat this?" },
            { 1, "How much do you plan to eat this in the future?" },
            { 2, "How healthy do you think this food is?" },
        };

        public static Dictionary<int, string> DailyReviewQuestions = new Dictionary<int, string>()
        {
            { 0, "Q1" },
            { 1, "Q2" },
            { 2, "Q3" },
            { 3, "Q4" },
            { 4, "Q5" },
            { 5, "Q6" },
            { 6, "Q7" },
        };

        public static string[] FrequencyResult =
        {
            "Never",
            "Rarely",
            "Sometimes",
            "Often",
            "Frequently"
        };

        public static string[] HealthResult =
        {
            "Very Unhealthy",
            "Unhealthy",
            "Somewhat Unhealthy",
            "Neutral",
            "Somewhat Healthy",
            "Healthy",
            "Very Healthy",
        };

        public enum EntryType
        {
            NEW_ENTRY = 0,
            UPDATE_ENTRY = 1
        }

        public static AspectRatio GetAspectRatio(int width, int height)
        {
            double aspectRatio = (double)Math.Max(width, height) / (double)Math.Min(width, height);
            if (aspectRatio == 16.0 / 9.0)
            {
                return AspectRatio.SixteenByNine;
            }
            return AspectRatio.FourByThree;
        }

        public static string SerializeDictionary<TKey, TValue>(Dictionary<TKey, TValue> dictionary)
        {
            StringBuilder builder = new StringBuilder();
            foreach (var pair in dictionary)
            {
                builder.Append(pair.Key).Append(":").Append(pair.Value).Append(',');
            }
            string result = builder.ToString();
            result = result.TrimEnd(',');
            return result;
        }

        public static Dictionary<int, int> DeserializeDictionary(string s)
        {
            Dictionary<int, int> d = new Dictionary<int, int>();
            string[] tokens = s.Split(new char[] { ':', ',' },
                StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < tokens.Length; i += 2)
            {
                string name = tokens[i];
                string freq = tokens[i + 1];

                int count = int.Parse(freq);
                int key = int.Parse(name);
                d.Add(key, count);
            }
            return d;
        }

        public static string SerializeByteArrayToString(byte[] bytes)
        {
            return Convert.ToBase64String(bytes);
        }

        public static byte[] DeserializeStringToByteArray(string str)
        {
            return Convert.FromBase64String(str);
        }

        public static Dictionary<DateTime, List<FoodItem>> getDateDiary()
        {
            return dateDiary;
        }

        public static List<FoodItem> getFoodItems()
        {
            return FoodItems;
        }

        public static bool RemoveFromDateDiary(FoodItem food)
        {
            DateTime date = food.DATE;
            if (dateDiary.ContainsKey(date.Date))
            {
                if (dateDiary[date.Date].Contains(food))
                {
                    int i = dateDiary[date.Date].IndexOf(food);
                    dateDiary[date.Date].RemoveAt(i);
                    if (dateDiary[date.Date].Count < 1) dateDiary.Remove(date.Date);
                }
            }
            return FoodItems.Remove(food);
        }

        async public static void UpdateDateDiary(int id)
        {
            List<FoodItem> foods = await foodItemsDatabase.QueryById(id);
            FoodItem food = foods.First();
            DateTime date = food.DATE;
            if (dateDiary.ContainsKey(date.Date))
            {
                if (dateDiary[date.Date].Contains(food))
                {
                    int i = dateDiary[date.Date].IndexOf(food);
                    dateDiary[date.Date].RemoveAt(i);
                }
                dateDiary[date.Date].Add(food);
            }
            else
            {
                dateDiary.Add(date.Date, new List<FoodItem>());
                dateDiary[date.Date].Add(food);
            }
            if (FoodItems.Contains(food))
            {
                int i = FoodItems.IndexOf(food);
                FoodItems[i] = food;
            }
            else
            {
                FoodItems.Add(food);
            }
        }

        async public static Task<Dictionary<DateTime, List<FoodItem>>> UpdateDateDiary()
        {
            dateDiary.Clear();
            FoodItems.Clear();
            List<FoodItem> foods = await foodItemsDatabase.QueryByDateTime();
            FoodItems = foods;
            foreach (var food in foods)
            {
                DateTime date = food.DATE;
                if (dateDiary.ContainsKey(date.Date))
                {
                    dateDiary[date.Date].Add(food);
                }
                else
                {
                    dateDiary.Add(date.Date, new List<FoodItem>());
                    dateDiary[date.Date].Add(food);
                }
            }
            return dateDiary;
        }
    }
}
