using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace SelfControl.Models
{
    [Table("DailyReview")]
    public class DailyReviewTable
    {
        static string DATETIME_COL = "_date";
        static string ID_COL = "_id";
        static string DAY_COL = "_day";
        static string ISCOMPLETED_COL = "_isCompleted";
        static string RESPONSE_COL = "_response";
        static string DATECREATED_COL = "_dateCreated";

        public static string dateTimeCol { get => DATETIME_COL; }
        public static string idCol { get => ID_COL; }
        public static string dayCol { get => DAY_COL; }
        public static string isCompletedCol { get => ISCOMPLETED_COL; }
        public static string responseCol { get => RESPONSE_COL; }
        public static string dateCreatedCol { get => DATECREATED_COL; }

        [PrimaryKey, AutoIncrement, Column("_id")]
        public int ID { get; set; }
        [Column("_date")]
        public DateTime DATE { get; set; }
        [Column("_dateCreated")]
        public DateTime DATECREATED { get; set; }
        [Column("_response")]
        public string RESPONSE { get; set; }
        [Column("_day")]
        public int DAY { get; set; }
        [Column("_isCompleted")]
        public bool ISCOMPLETED { get; set; }
    }
}
