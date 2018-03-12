using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace SelfControl.Models
{
    [Table("FoodItems")]
    public class FoodItem
    {
        static string DATETIME_COL = "_date";
        static string ID_COL = "_id";
        static string NAME_COL = "_name";
        static string PATH_COL = "_path";
        static string COOL_COL = "_coolEffect";
        static string HOT_COL = "_hotEffect";
        static string IMG_HEIGHT = "_height";
        static string IMG_WIDTH = "_width";
        static string IMG_ORIENTATION = "_orientation";
        static string FREQUENCY_COL = "_frequency";
        static string HEALTH_COL = "_health";
        static string PLAN_COL = "_plan";

        public static string dateTimeCol { get => DATETIME_COL; }
        public static string idCol { get => ID_COL; }
        public static string nameCol { get => NAME_COL; }
        public static string pathCol { get => PATH_COL; }
        public static string coolCol { get => COOL_COL; }
        public static string hotCol { get => HOT_COL; }
        public static string widthCol { get => IMG_WIDTH; }
        public static string hieghtCol { get => IMG_HEIGHT; }
        public static string orientationCol { get => IMG_ORIENTATION; }
        public static string frequencyCol { get => FREQUENCY_COL; }
        public static string healthCol { get => HEALTH_COL; }
        public static string planCol { get => PLAN_COL; }

        [PrimaryKey, AutoIncrement, Column("_id")]
        public int ID { get; set; }
        [Column("_date")]
        public DateTime DATE { get; set; }
        [Column("_name")]
        public string NAME { get; set; }
        [Column("_path")]
        public string PATH { get; set; }
        [Column("_coolEffect")]
        public bool COOLEFFECT { get; set; }
        [Column("_hotEffect")]
        public bool HOTEFFECT { get; set; }
        [Column("_height")]
        public int IMGHEIGHT { get; set; }
        [Column("_width")]
        public int IMGWIDTH { get; set; }
        [Column("_orientation")]
        public int IMGORIENTATION { get; set; }
        [Column("_freqeuncy")]
        public int FREQUENCY { get; set; }
        [Column("_health")]
        public int HEALTH { get; set; }
        [Column("_plan")]
        public int PLAN { get; set; }
    }
}
