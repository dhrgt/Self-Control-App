using System;
using System.Collections.Generic;
using System.Text;

namespace SelfControl.Interfaces
{
    public interface IFileHelper
    {
        string GetLocalFilePath(string filename);
        string GetExternalFilePath(string filename);
        bool deleteFile(string path);
    }
}
