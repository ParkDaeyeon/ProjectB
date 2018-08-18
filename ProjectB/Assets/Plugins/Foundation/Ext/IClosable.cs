using System;
using System.Collections.Generic;
namespace Ext
{
    public interface IClosable
    {
        bool IsOpened { get; }
        void Open();
        void Close();
    }
}
