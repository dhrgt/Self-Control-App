using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SelfControl.DatabaseManager
{
    class DatabaseQuery
    {
        static string ORDER_BY_DATETIME = "SELECT * FROM FoodItems ORDER BY ?";

        public static string orderByDateTime { get => ORDER_BY_DATETIME; }
    }
}
