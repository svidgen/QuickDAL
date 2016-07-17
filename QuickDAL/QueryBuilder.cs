using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuickDAL
{
    public interface QueryBuilder : IDisposable
    {
        List<T> Find<T>(
            List<DataObject> conditions,
            Boolean fuzzy = false,
            String order = null,
            Int32 limit = Int32.MaxValue,
            T start = null
        ) where T : DataObject, new();

        List<T> Find<T>(DataObject condition, Boolean fuzzy = false,
            String order = null,
            Int32 limit = Int32.MaxValue,
            T start = null
        ) where T : DataObject, new();

        List<T> Find<T>() where T : DataObject, new();
        Int32 Save<T>(T o, Boolean fullUpdate = false) where T : DataObject, new();
        Int32 Delete<T>(T o) where T : DataObject, new();
    }
}
