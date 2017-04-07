# Blessing an Entity

Telling QuickDAL how your entity relates to table in a database is the hardest part.

Most basically, your classes will inherit from [DataObject](../master/QuickDAL/DataObject.cs) and provide the [DataDefinition](../master/QuickDAL/DataDefinition.cs) and configured [QueryBuilder](../master/QuickDAL/QueryBuilder.cs) needed to relate the object to a table in the database. QuickDAL intentionally avoids reflection and aims for high flexibility, so these "configuration" objects live at the instance level.

## For example

```c#
public class Order : DataObject<Order>
{
	public Guid OrderID { get; set; }
	public String Address { get; set; }
	/* etc. */

	// where the object lives
	public override QueryBuilder GetQueryBuilder()
	{
		return ConfigurationService.GetQueryBuilder();
	}

	// how the object relates to the schema
	public override DataDefinition GetDefinition()
	{
		return new DataDefinition()
		{
			DataEntity = "order",
			PrimaryKey = "orderID",
			Maps = new Dictionary<String, IReference>()
			{
				/* {"db-field-name", new Refrence<Type>(
					   // getter
					   () => LocalField,
					   // setter
					   (v) => LocalField = v
				)} */
				{"Address", new Reference<String>(() => Address, v => Address = v)},
				{"orderID", new Reference<Guid>(() => Id, v => Id = v)},
			}
		};
	}
}
```

Once this is done, you can leverage the QuickDAL [CRUD Methods](crud.md) on your `Order` entity.

To use other entities as query parameters, learn about [Building Relationships](relationships.md).

