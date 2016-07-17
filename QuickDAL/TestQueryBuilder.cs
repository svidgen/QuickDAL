using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuickDAL
{
    public class TestQueryBuilder : QueryBuilder
    {

        private static Dictionary<String, Object> Tables = new Dictionary<String, Object>();

        private TransactionalCollection<T> GetCollection<T>() where T : DataObject, new()
        {
            return GetCollection(new T());
        }

        private TransactionalCollection<T> GetCollection<T>(T o) where T : DataObject, new()
        {
            var collectionName = o.GetDefinition().DataEntity;
            if (!Tables.ContainsKey(collectionName))
            {
                Tables[collectionName] = new TransactionalCollection<T>();
            }
            return (TransactionalCollection<T>)Tables[collectionName];
        }

        public int DeleteT<T>(T o) where T : DataObject, new()
        {
            var collection = GetCollection(o);
            var removed = collection.Remove(o);

            if(removed)
            {
                return 1;
            } else
            {
                return 0;
            }
        }

        public List<T> Find<T>() where T : DataObject, new()
        {
            var collection = GetCollection<T>();
            return collection.ToList();
        }

        public List<T> Find<T>(DataObject condition, bool fuzzy = false, string order = null, int limit = int.MaxValue, T start = null) where T : DataObject, new()
        {
            throw new NotImplementedException();
        }

        public List<T> Find<T>(List<DataObject> conditions, bool fuzzy = false, string order = null, int limit = int.MaxValue, T start = null) where T : DataObject, new()
        {
            // naive solution...

            // start with all the stuff ...
            var rv = new List<T>();
            var rv_sample = (new T());

            // iteratively remove items that don't have matches.
            conditions.GroupBy(c => c.GetDefinition().DataEntity).ToList()
                .ForEach(g => rv.AddRange(v => RecursiveSelect<T>(g.ToList())))
            ;

            return rv;
        }

        private List<T> RecursiveSelect<T>(List<DataObject> conditions) where T : DataObject, new()
        {
            var joinpath = new JoinPath((new T()), conditions.First());
            var collection = (ICollection<Object>)Tables[conditions.First().GetDefinition().DataEntity];

            var conditionMatchingRecords = collection.Where(record => conditions.Any(c => Matches((DataObject)record, c))).ToList();

            if (joinpath.Relationships.Count == 0)
            {
                return conditionMatchingRecords.Select(r => (T)r).ToList();
            } else
            {
                // ???
            }

        }

        private Boolean Matches(DataObject o, DataObject condition)
        {
            var _o = o.ToDictionary();
            var _condition = condition.ToDictionary();
            foreach (var kv in _condition)
            {
                if (!_o[kv.Key].Equals(kv.Value))
                {
                    return false;
                }
            }
            return true;
        }

        public int Save<T>(T o, bool fullUpdate = false) where T : DataObject, new()
        {
            var collection = GetCollection(o);
            collection.Add(o);
            return 1;
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
