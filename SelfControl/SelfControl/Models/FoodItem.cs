using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace SelfControl.Models
{
    [Table("FoodItems")]
    public class FoodItem
    {
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
    }
}
