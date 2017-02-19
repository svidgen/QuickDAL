# CRUD

When a class inherits from `DataObject` like so:

```c#
public LineItem : DataObject<LineItem>
{
	public Guid LineItemId { get; set; }
	/* etc. */
}
```

... it gets *blessed* with three CRUDdy methods.

## `static Get()`

Given an instance of the entities PK, this returns a single record. Given a `DataObject` or `List<DataObject>` as criteria, it returns `List<T>`.

```c#
var lines = LineItem.Get(new LineItem() {
	OrderId = new Guid("{622E2962-2E91-4105-A01B-B6C897E38420}")
});
```

For example:

* *`Get({Guid})`* : Returns a single record by GUID PK. If the class doesn't use GUID PK's, `null` is returned.
* *`Get(DataObject condition)`* : Returns multiple objects based on the query object's populated values.
* *`Get(List<DataObject> conditions)`* : Returns multiple objects based on multiple conditions. Multiple conditions with the same types are treated as `OR` conditions. Conditions of different types are treated as `AND` conditions.
* *`Get(condition(s), Boolean fuzzy, String order, Int32 limit, T start)`*
** `fuzzy`: Whether `LIKE` is used instead of `=` when searching
** `order`: An order-by string to insert in the `order by X` clause
** `limit`: The maximum number of rows to return
** `start`: A record indicating which record to _start_ returning after. It should matche the return type and have the `order` (or default order) field populated. Very useful for building efficient result-paging.

QuickDAL will find the quickest path from your conditions to your return-type when assembling a query. (Explicit, query-time paths are not yet supported, but they're coming!)

## `Save()`

Saves the record. Performs an `UPDATE` if the PK is populated; otherwise and `INSERT`. The PK on the object is populated in the case of an `INSERT`.

For example:

```c#
var line = new LineItem() {
	OrderId = new Guid("{622E2962-2E91-4105-A01B-B6C897E38420}")
};
line.Save();
line.LineItemId;	// now populated with a newly generated ID
```

## `Detele()`

Deletes records. If the PK is populated on the target, this will delete a single records. But, it also supported deleting multiple records when the PK is left empty.

```c#
var line = new LineItem() {
	OrderId = new Guid("{622E2962-2E91-4105-A01B-B6C897E38420}")
};
line.Delete();	// deletes ALL lines under the order.
```
