using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SampleDomain.Services;

namespace SampleDomain
{
    public class Customer : Persistent<Customer>
    {
        public Guid CustomerId { get; set; }
        public String FirstName { get; set; }
        public String LastName { get; set; }

        private List<SalesOrder> _Orders;
        public List<SalesOrder> Orders
        {
            get
            {
                if (_Orders == null)
                {
                    _Orders = SalesOrder.Get(this);
                }
                return _Orders;
            }
            set
            {
                _Orders = value;
            }
        }

        private List<Product> _Products;
        public List<Product> Products
        {
            get
            {
                if (_Products == null)
                {
                    _Products = Product.Get(this);
                }
                return _Products;
            }
            set
            {
                _Products = value;
            }
        }

        public override DataDefinition GetPersistenceDefinition()
        {
            return new DataDefinition()
            {
                DataEntity = "Customers",
                PrimaryKey = "CustomerId",
                Maps = new DataMap()
                {
                    {"CustomerId", new Reference<Guid>(() => CustomerId, (v) => CustomerId = v)},
                    {"FirstName", new Reference<String>(() => FirstName, (v) => FirstName = v)},
                    {"LastName", new Reference<String>(() => LastName, (v) => LastName = v)},
                }
            };
        }

    }
}
