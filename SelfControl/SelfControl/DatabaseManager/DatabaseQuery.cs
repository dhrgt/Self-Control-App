using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SelfControl.DatabaseManager
{
    class DatabaseQuery
    {
        static string ORDER_BY_DATETIME = "select * from Table order by datetime(?) DESC";

        public static string orderByDateTime { get => ORDER_BY_DATETIME; }
    }
}
