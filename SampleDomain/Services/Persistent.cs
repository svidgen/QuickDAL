using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuickDAL;

namespace SampleDomain.Services
{
    public abstract class Persistent<T> : DataObject<T> where T : DataObject<T>, new()
    {
        private static QueryBuilder _QueryBuilder = null;
        public override QueryBuilder GetQueryBuilder()
        {
            if (_QueryBuilder == null)
            {
                _QueryBuilder = new TestQueryBuilder();
            }
            return _QueryBuilder;
        }

        public abstract DataDefinition GetPersistenceDefinition();

        public override QuickDAL.DataDefinition GetDefinition()
        {
            return GetPersistenceDefinition();
        }

    }

    public class DataDefinition : QuickDAL.DataDefinition {}
    public class DataMap : Dictionary<String, QuickDAL.IReference> {}
    public interface IReference : QuickDAL.IReference {}

    public class Reference : QuickDAL.Reference, IReference
    {
        public Reference(Func<Object> Getter, Action<Object> Setter, Type type = null)
            : base(Getter, Setter, type) {}
    }

    public class Reference<T> : QuickDAL.Reference<T>, IReference
    {
        public Reference(Func<T> Getter, Action<T> Setter)
            : base(Getter, Setter) {}
    }

}
