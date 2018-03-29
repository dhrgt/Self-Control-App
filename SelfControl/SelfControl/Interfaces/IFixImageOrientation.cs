using System;
using System.Collections.Generic;
using System.Text;

namespace SelfControl.Interfaces
{
    public interface IFixImageOrientation
    {
        byte[] fixOrientation(byte[] bytes);
    }
}
