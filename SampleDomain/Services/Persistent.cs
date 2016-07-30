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

    }
}
