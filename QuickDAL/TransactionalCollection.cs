using System;
using System.Collections;
using System.Collections.Generic;
using System.Transactions;
using System.Linq;
using System.Text;

namespace QuickDAL
{
    public class TransactionalCollection<T> : ICollection<T>, IEnlistmentNotification where T : DataObject, new()
    {

        private String PK;
        private Dictionary<String, T> Items = new Dictionary<String, T>();
        private Dictionary<String, T> RollbackItems = new Dictionary<String, T>();

        public TransactionalCollection()
        {
            var sample = new T();
            PK = sample.GetDefinition().PrimaryKey;
        }

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

        private String GetObjectKey(T value)
        {
            return value.ToDictionary()[PK];
        }

        public void Add(T item)
        {
            var key = GetObjectKey(item);

            if (JoinCurrentTransaction() && !RollbackItems.ContainsKey(key))
            {
                if (Items.ContainsKey(key))
                {
                    RollbackItems[key] = Items[key];
                }
                else
                {
                    RollbackItems[key] = null;
                }
            }
        }

        public bool Remove(T item)
        {
            var key = GetObjectKey(item);
            if (!Items.ContainsKey(key))
            {
                return false;
            }

            if (JoinCurrentTransaction() && !RollbackItems.ContainsKey(key))
            {
                RollbackItems[key] = Items[key];
            }
            return Items.Remove(key);
        }

        public void Clear()
        {
            if (JoinCurrentTransaction())
            {
                foreach (var kv in Items)
                {
                    RollbackItems.Add(kv.Key, kv.Value);
                }
            }
            Items.Clear();
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

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Items.GetEnumerator();
        }

        #endregion IList<T>

        #region IEnlistmentNotification + Transactional helpers

        private Boolean JoinCurrentTransaction()
        {
            var currentTransaction = Transaction.Current;
            if (currentTransaction != null)
            {
                currentTransaction.EnlistVolatile(this, EnlistmentOptions.None);
                return true;
            }
            return false;
        }

        public void Commit(Enlistment enlistment)
        {
            RollbackItems.Clear();
        }

        public void InDoubt(Enlistment enlistment)
        {
            // no InDoubt() action
        }

        public void Prepare(PreparingEnlistment preparingEnlistment)
        {
            preparingEnlistment.Prepared();
        }

        public void Rollback(Enlistment enlistment)
        {
            foreach (var kv in RollbackItems)
            {
                if (kv.Value == null)
                {
                    Items.Remove(kv.Key);
                }
                else
                {
                    Items[kv.Key] = kv.Value;
                }
            }
            RollbackItems.Clear();
        }

        #endregion IEnlistmentNotification


    }
}
