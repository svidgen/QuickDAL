using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuickDAL
{
    public class TestQueryBuilder : QueryBuilder
    {

        private static Dictionary<String, TransactionalCollection<DataObject>> Tables = new Dictionary<String, TransactionalCollection<DataObject>>();

        private TransactionalCollection<DataObject> GetCollection<T>() where T : DataObject, new()
        {
            return GetCollection(new T());
        }

        private TransactionalCollection<DataObject> GetCollection(DataObject o)
        {
            var collectionName = o.GetDefinition().DataEntity;
            if (!Tables.ContainsKey(collectionName))
            {
                Tables[collectionName] = new TransactionalCollection<DataObject>();
            }
            return Tables[collectionName];
        }

        public int Delete(DataObject o)
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
            return collection.Select(o => (T)o).ToList();
        }

        public List<T> Find<T>(DataObject condition, bool fuzzy = false, string order = null, int limit = int.MaxValue, T start = null) where T : DataObject, new()
        {
            return Find<T>(new List<DataObject>() { condition }, fuzzy, order, limit, start);
        }

        public List<T> Find<T>(List<DataObject> conditions, bool fuzzy = false, string order = null, int limit = int.MaxValue, T start = null) where T : DataObject, new()
        {
            // naive solution...

            var rv = new List<T>();
            var PK = (new T()).GetDefinition().PrimaryKey;

            // group the conditions by type/table
            var conditionGroups = conditions.GroupBy(c => c.GetDefinition().DataEntity).ToList();

            // the first group will be used to form the base set.
            var baseConditions = conditionGroups.First().ToList();
            rv.AddRange(RecursiveSelect<T>(baseConditions));
            conditionGroups.RemoveAt(0);

            // subsequent conditions serve to eliminate rows.                
            while (conditionGroups.Count > 0)
            {
                var joinConditions = conditionGroups.First().ToList();
                var joinRecords = RecursiveSelect<T>(joinConditions);
                var joinRecordPKs = joinRecords.Select(r => r.ToDictionary()[PK]).ToList();
                rv = rv.Where(row => joinRecordPKs.Contains(row.ToDictionary()[PK])).ToList();
                conditionGroups.RemoveAt(0);
            }

            return rv;
        }

        private List<T> RecursiveSelect<T>(List<DataObject> conditions) where T : DataObject, new()
        {
            var joinpath = new JoinPath((new T()), conditions.First());
            var collection = GetCollection(conditions.First());

            var rightHandRecords = collection.Where(record => conditions.Any(c => Matches(record, c))).ToList();

            if (joinpath.Relationships.Count == 0)
            {
                // the selected records ARE the rows we're looking for.
                return rightHandRecords.Select(r => (T)r).ToList();
            }

            var nextJoin = joinpath.Relationships.Last();

            var joinEntityLeft = nextJoin.LocalEntity;
            var joinFieldLeft = nextJoin.LocalField;
            var joinFieldRight = nextJoin.RemoteField;

            var newConditions = rightHandRecords.Select(r => {
                var newCondition = joinEntityLeft.Copy();
                newCondition.Populate(joinFieldLeft, r.ToDictionary()[joinFieldRight]);
                return newCondition;
            }).ToList();

            return RecursiveSelect<T>(newConditions);
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

        public int Save(DataObject o, bool fullUpdate = false)
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

