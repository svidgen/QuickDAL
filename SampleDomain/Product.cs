using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SampleDomain.Services;

namespace SampleDomain
{
    public class Product : Persistent<Product>
    {
        public Guid ProductId { get; set; }
        public String Name { get; set; }
        public Decimal UnitPrice { get; set; }

        public override DataDefinition GetPersistenceDefinition()
        {
            return new DataDefinition()
            {
                DataEntity = "Products",
                PrimaryKey = "ProductId",
                Maps = new DataMap()
                {
                    {"ProductId", new Reference<Guid>(() => ProductId, (v) => ProductId = v)},
                    {"Name", new Reference<String>(() => Name, (v) => Name = v)},
                    {"UnitPrice", new Reference<Decimal>(() => UnitPrice, (v) => UnitPrice = v)},
                }
            };
        }
    }
}
