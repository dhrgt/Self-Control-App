using Plugin.Settings;
using Plugin.Settings.Abstractions;

namespace SelfControl.Helpers
{
    public static class Settings
    {
        private static ISettings AppSettings
        {
            get
            {
                return CrossSettings.Current;
            }
        }

        #region Setting Constants

        private const string StageSetting = "stage_key";
        private static readonly int StageSettingDefault = GlobalVariables.STAGE_1;

        private const string CameraScaleSetting = "camera_scale";
        private static readonly double CameraScaleSettingDefault = GlobalVariables.CAMERA_PREVIEW_SCALE;

        private const string CameraRoiValueKey = "camera_roi_value_key";
        private static readonly double CameraRoiValueDefault = GlobalVariables.CAMERA_ROI_SIZE;

        private const string RandomCriteriaSetting = "random_criteria_setting";
        private static readonly int RandomCriteriaSettingDefault = (int)GlobalVariables.RandomCriteria.Random;

        private const string LastDailyReviewSetting = "last_daily_review_setting";
        private static readonly System.DateTime LastDailyReviewSettingDefault = System.DateTime.Now;

        private const string FirstDailyReviewSetting = "first_daily_review_setting";
        private static readonly bool FirstDailyReviewSettingDefault = false;

        private const string DailyReviewDaySetting = "daily_review_day_setting";
        private static readonly int DailyReviewDaySettingDefault = 0;

        #endregion


        public static int StageSettings
        {
            get
            {
                return AppSettings.GetValueOrDefault(StageSetting, StageSettingDefault);
            }
            set
            {
                AppSettings.AddOrUpdateValue(StageSetting, value);
            }
        }

        public static double CameraScaleSettings
        {
            get
            {
                return AppSettings.GetValueOrDefault(CameraScaleSetting, CameraScaleSettingDefault);
            }
            set
            {
                AppSettings.AddOrUpdateValue(CameraScaleSetting, value);
            }
        }

        public static double CameraRoiValue
        {
            get
            {
                return AppSettings.GetValueOrDefault(CameraRoiValueKey, CameraRoiValueDefault);
            }
            set
            {
                AppSettings.AddOrUpdateValue(CameraRoiValueKey, value);
            }
        }

        public static int RandomCriteriaValue
        {
            get
            {
                return AppSettings.GetValueOrDefault(RandomCriteriaSetting, RandomCriteriaSettingDefault);
            }
            set
            {
                AppSettings.AddOrUpdateValue(RandomCriteriaSetting, value);
            }
        }

        public static System.DateTime LastDailyReviewValue
        {
            get
            {
                return AppSettings.GetValueOrDefault(LastDailyReviewSetting, LastDailyReviewSettingDefault);
            }
            set
            {
                AppSettings.AddOrUpdateValue(LastDailyReviewSetting, value);
            }
        }

        public static bool FirstDailyReviewValue
        {
            get
            {
                return AppSettings.GetValueOrDefault(FirstDailyReviewSetting, FirstDailyReviewSettingDefault);
            }
            set
            {
                AppSettings.AddOrUpdateValue(FirstDailyReviewSetting, value);
            }
        }

        public static int DailyReviewDayValue
        {
            get
            {
                return AppSettings.GetValueOrDefault(DailyReviewDaySetting, DailyReviewDaySettingDefault);
            }
            set
            {
                AppSettings.AddOrUpdateValue(DailyReviewDaySetting, value);
            }
        }
    }
}