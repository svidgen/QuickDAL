# Building Relationships

Relationships maps can be placed into an object's DataDefinition to instruct QuickDAL in building `JOIN`'s.

Relationships can be specified as either *Parent* or *Child* relationships. In the current revision QuickDAL, these can be used interchangeabley -- and we currently plan on squashing these together for simplicity. So, we suggest using only `Child` relationships for now.

## For example

A `SalesOrder` might have the following `DataDefinition`:

```c#
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
		},
		Parents = new RelationshipCollection()
		{
			new DataRelationship<SalesOrder, Customer>("CustomerId")
		},
		Children = new RelationshipCollection()
		{
			new DataRelationship<SalesOrder, LineItem)("SalesOrderId")
		},
	};
}
```

The `QueryBuilder` begins searching for possible `JOIN`'s *starting* with `this` and moving outwards. So, the relationships on `SalesOrder` above faciliate finding `SalesOrder`'s using `LineItem`'s or `Customer`'s as conditions:

```c#
// find all orders for Jon Wire
var orders = SalesOrder.Get(new Customer() {
	FirstName = "Jon",
	LastName = "Wire"
});

// find the order for a particular line item
var orders = SalesOrder.Get(new LineItem() { LineItemId = 123 });

// find all orders for all lines for a particular SKU
var orders = SalesOrder.Get(new LineItem() { SKU = "SOMESKU" });
```
	
Etc.
