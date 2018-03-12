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

        public const string IMAGE_PAGE_QUESTION = "Do you want to eat more of this?";

        public const string DATABASE_NAME = "foodDB.db3";

        public enum AspectRatio
        {
            FourByThree = 1,
            SixteenByNine = 2
        }

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
    }
}
