using SelfControl.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SelfControl.DatabaseManager
{
    class DatabaseQuery
    {
        static string ORDER_BY_DATETIME = "SELECT * FROM FoodItems ORDER BY ?";
        static string GET_ITEM_BY_ID = "SELECT * FROM FoodItems WHERE " + FoodItem.idCol + " = ?";
        static string GET_ID_BY_DATE = "SELECT " + FoodItem.idCol + " FROM FoodItems WHERE " + FoodItem.dateTimeCol + " = ?";

        public static string orderByDateTime { get => ORDER_BY_DATETIME; }
        public static string getItemById { get => GET_ITEM_BY_ID; }
        public static string getIdByDate { get => GET_ID_BY_DATE; }
    }
}
