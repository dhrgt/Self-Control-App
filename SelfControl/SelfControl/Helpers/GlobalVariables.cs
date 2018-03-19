using System;
using System.Collections.Generic;
using System.Text;

namespace SelfControl.Helpers
{
    public class GlobalVariables
    {
        public const int STAGE_1 = 1;
        public const int STAGE_2 = 2;

        public const int SIZE_OF_FOOD_LIBRARY = 3;

        public const double CAMERA_PREVIEW_SCALE = 1.0;

        public const double CAMERA_ROI_SIZE = 5.0;

        public const string DATABASE_NAME = "foodDB.db3";

        public enum AspectRatio
        {
            FourByThree = 1,
            SixteenByNine = 2
        }

        public static Dictionary<int, string> Questions = new Dictionary<int, string>()
        {
            { 0, "How frequently do you eat this?" },
            { 1, "How much do you plan to eat this in the future?" },
            { 2, "How healthy do you think this food is?" }
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
    }
}
