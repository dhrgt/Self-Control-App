using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace SelfControl.Models
{
    [Table("WeeklyReview")]
    public class WeeklyReviewTable
    {
        static string DATETIME_COL = "_date";
        static string ID_COL = "_id";
        static string PIC_ID = "_picId";
        static string RESPONSE_COL = "_response";

        public static string dateTimeCol { get => DATETIME_COL; }
        public static string idCol { get => ID_COL; }
        public static string picIdCol { get => PIC_ID; }
        public static string responseCol { get => RESPONSE_COL; }

        [Column("_id")]
        public int ID { get; set; }
        [Column("_picId")]
        public int PICID { get; set; }
        [Column("_date")]
        public DateTime DATE { get; set; }
        [Column("_response")]
        public string RESPONSE { get; set; }
    }
}
