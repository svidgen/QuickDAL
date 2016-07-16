using System;
using System.Collections;
using System.Collections.Generic;
using System.Transactions;
using System.Linq;
using System.Text;

namespace QuickDAL
{
    public class TransactionalCollection<T> : ICollection<T>, IEnlistmentNotification
    {

        private Dictionary<String, T> Items = new Dictionary<String, T>();
        private Dictionary<String, T> RollbackItems = new Dictionary<String, T>();

        #region ICollection<T>

        public int Count
        {
            get
            {
                return Items.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        public void Add(T item)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(T item)
        {
            return Items.ContainsValue(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            Items.Values.ToList().CopyTo(array, arrayIndex);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return Items.Values.GetEnumerator();
        }

        public bool Remove(T item)
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Items.GetEnumerator();
        }

        #endregion IList<T>

        #region IEnlistmentNotification + Transactional helpers

        private void JoinCurrentTransaction()
        {
            var currentTransaction = Transaction.Current;
            if (currentTransaction != null)
            {
                currentTransaction.EnlistVolatile(this, EnlistmentOptions.None);
            }
        }

        public void Commit(Enlistment enlistment)
        {
            throw new NotImplementedException();
        }
        
        public void InDoubt(Enlistment enlistment)
        {
            throw new NotImplementedException();
        }

        public void Prepare(PreparingEnlistment preparingEnlistment)
        {
            throw new NotImplementedException();
        }
        
        public void Rollback(Enlistment enlistment)
        {
            throw new NotImplementedException();
        }

        #endregion IEnlistmentNotification


    }
}
