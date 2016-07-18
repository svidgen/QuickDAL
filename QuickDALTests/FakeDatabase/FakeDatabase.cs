using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuickDAL;
using System.Data;

namespace QuickDALTests.FakeDatabase
{
    public class FakeDatabase
    {

        public QueryBuilder GetQueryBuilder()
        {
            return new QueryBuilder(
                createCommand,
                executeReader,
                executeNonQuery,
                executeScalar
            );
        }

        private FakeDbCommand createCommand()
        {
            return new FakeDbCommand();
        }

        private FakeDataReader executeReader(IDbCommand query)
        {
            return new FakeDataReader();
        }

        private Int32 executeNonQuery(IDbCommand query)
        {
            return new Int32();
        }

        private Object executeScalar(IDbCommand query)
        {
            return new Object();
        }

    }
}
