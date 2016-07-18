using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuickDAL;

namespace QuickDALTests.SampleClasses
{
    public class Product : DataObject<Product>
    {
        public Guid ProductId { get; set; }
        public String Name { get; set; }
        public Decimal UnitPrice { get; set; }

        public override QueryBuilder GetQueryBuilder()
        {
            return QueryBuilderLocator.GetQueryBuilder();
        }

        public override DataDefinition GetDefinition()
        {
            return new DataDefinition()
            {
                DataEntity = "Products",
                PrimaryKey = "ProductId",
                Maps = new Dictionary<string, IReference>()
                {
                    {"ProductId", new Reference<Guid>(() => ProductId, (v) => ProductId = v)},
                    {"Name", new Reference<String>(() => Name, (v) => Name = v)},
                    {"UnitPrice", new Reference<Decimal>(() => UnitPrice, (v) => UnitPrice = v)},
                }
            };
        }
    }
}
