using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuickDAL;

namespace QuickDALTests
{
    public class QueryBuilderLocator
    {
        private static QueryBuilder _QueryBuilder;
        public static QueryBuilder GetQueryBuilder()
        {
            if (_QueryBuilder == null)
            {
                _QueryBuilder = new TestQueryBuilder();
            }
            return _QueryBuilder;
        }
    }
}
