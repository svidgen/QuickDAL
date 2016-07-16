using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuickDAL
{
    public interface ICacheCollection
    {
        Object this[String key]  {  get; set;  }

        void Clear();

    }
}
