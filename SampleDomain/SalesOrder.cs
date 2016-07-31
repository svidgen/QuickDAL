using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SampleDomain.Services;

namespace SampleDomain
{
    public class SalesOrder : Persistent<SalesOrder>
    {

        public Guid SalesOrderId { get; set; }
        public DateTime DateSubmitted { get; set; }
        private Guid CustomerId { get; set; }

        private Customer _Customer { get; set; }
        public Customer Customer {
            get
            {
                if (_Customer == null || !_Customer.CustomerId.Equals(CustomerId))
                {
                    _Customer = Customer.Get(CustomerId);
                }
                return _Customer;
            }
            set
            {
                _Customer = value;
                CustomerId = Guid.Empty;
                if (_Customer != null)
                {
                    CustomerId = _Customer.CustomerId;
                }
            }
        }

        public override DataDefinition GetPersistenceDefinition()
        {
            return new DataDefinition()
            {
                DataEntity = "SalesOrders",
                PrimaryKey = "SalesOrderId",
                Maps = new DataMap()
                {
                    {"SalesOrderId", new Reference<Guid>(() => SalesOrderId, (v) => SalesOrderId = v)},
                    {"DateSubmitted", new Reference<DateTime>(() => DateSubmitted, (v) => DateSubmitted = v)},
                    {"CustomerId", new Reference<Guid>(() => CustomerId, (v) => CustomerId = v)},
                }
            };
        }
    }
}
