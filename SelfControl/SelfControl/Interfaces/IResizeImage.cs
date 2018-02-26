using System;
using System.Collections.Generic;
using System.Text;

namespace SelfControl.Interfaces
{
    public interface IResizeImage
    {
        byte[] Resize(byte[] image, int width, int height);
    }
}
