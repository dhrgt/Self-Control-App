using SelfControl.Interfaces;
using SelfControl.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using SQLite;
using SelfControl.Models;
using System.Threading.Tasks;

namespace SelfControl.DatabaseManager
{
    public class ConnectionManager
    {
        protected readonly SQLiteAsyncConnection database;

        public ConnectionManager(string filePath)
        {
            database = new SQLiteAsyncConnection(filePath);
           
        }
    }
}
