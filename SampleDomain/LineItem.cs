using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SampleDomain.Services;

namespace SampleDomain
{
    public class LineItem : Persistent<LineItem>
    {
        public Guid LineItemId { get; set; }
        private Guid SalesOrderId { get; set; }
        private Guid ProductId { get; set; }
        public Int32 Quantity { get; set; }


        private SalesOrder _SalesOrder;
        public SalesOrder SalesOrder
        {
            get
            {
                if (_SalesOrder == null || !_SalesOrder.SalesOrderId.Equals(SalesOrderId))
                {
                    _SalesOrder = SalesOrder.Get(SalesOrderId);
                }
                return _SalesOrder;
            }
            set
            {
                _SalesOrder = value;
                SalesOrderId = Guid.Empty;
                if (_SalesOrder != null)
                {
                    SalesOrderId = value.SalesOrderId;
                }
            }
        }

        private Product _Product;
        public Product Product
        {
            get
            {
                if (_Product == null || !_Product.ProductId.Equals(ProductId))
                {
                    _Product = Product.Get(ProductId);
                }
                return _Product;
            }
            set
            {
                _Product = value;
                ProductId = Guid.Empty;
                if (_Product != null)
                {
                    ProductId = value.ProductId;
                }
            }
        }

        public override DataDefinition GetPersistenceDefinition()
        {
            return new DataDefinition()
            {
                DataEntity = "LineItems",
                PrimaryKey = "LineItemId",
                Maps = new DataMap()
                {
                    {"LineItemId", new Reference<Guid>(() => LineItemId, (v) => LineItemId = v)},
                    {"SalesOrderId", new Reference<Guid>(() => SalesOrderId, (v) => SalesOrderId = v)},
                    {"ProductId", new Reference<Guid>(() => ProductId, (v) => ProductId = v)},
                    {"Quantity", new Reference<Int32>(() => Quantity, (v) => Quantity = v)},
                },
                Parents = new RelationshipCollection()
                {
                    new DataRelationship<LineItem, SalesOrder>("SalesOrderId")
                }
            };
        }
    }
}
