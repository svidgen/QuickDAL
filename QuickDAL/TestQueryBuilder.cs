using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuickDAL
{
    public class TestQueryBuilder : QueryBuilder
    {

        private static Dictionary<String, TransactionalCollection> Tables = new Dictionary<String, TransactionalCollection>();


        public int Delete(DataObject o)
        {
            throw new NotImplementedException();
        }

        public List<T> Find<T>() where T : DataObject, new()
        {
            throw new NotImplementedException();
        }

        public List<T> Find<T>(DataObject condition, bool fuzzy = false, string order = null, int limit = int.MaxValue, T start = null) where T : DataObject, new()
        {
            throw new NotImplementedException();
        }

        public List<T> Find<T>(List<DataObject> conditions, bool fuzzy = false, string order = null, int limit = int.MaxValue, T start = null) where T : DataObject, new()
        {
            throw new NotImplementedException();
        }

        public int Save(DataObject o, bool fullUpdate = false)
        {
            throw new NotImplementedException();
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
