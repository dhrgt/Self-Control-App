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
    }
}