# ☠️ ⚠️ ☠️ WARNING!!! ☠️ ⚠️ ☠️

This project is **archived** and **dangerous**!

QuickDAL was originally created to provide an abstraction over SQL on a project that proved difficult to migrate to other ORM's _at the time_. QuickDAL was an in-house solution developed to bridge the gap. It sufficiently fulfilled that role, but contains a critical "feature" that should never ever ever _ever_ be used in a production codebase **as-is**.

Namely, this DAL allows you to query for model `A` using criteria from model `Z` without specifying the specific relationship/join path. QuickDAL will instead "search" for and use the most likely intended path from `A` to `Z` (usually the shortest-path). Maybe this is a cool feature for some geeky projects. For real-world applications, this is a terrible, **terrible** feature. **_DO NOT USE IT_.**

Feel free to pick this up, play with it, and learn from it. But, do not use it as-is in production.

## If I had to do it again

I would definitely forgo the implicit join logic, saving me tons of mental energy and proably reducing the codebase by around [80%](https://en.wikipedia.org/wiki/Pareto_principle).

If you're out there, creating your own DAL or ORM, be explicit! 😅

# QuickDAL

A very small library that provides a simple and efficient Data Access Layer between business entities and a T-SQL compatible database.

Entities that inherit from QuickDAL's `DataObject` class and inform QuickDAL about how they relate to the database can be queried effortlessly.

## The simple
Assume we have a `Product` entity that has been properly mapped. Writing queries against that entity is simple and straightforward.

### Getting a single record with its PK

```c#
Guid id = GetProductIdFromRequest();
Product p = Product.Get(id);
```

### Finding records using an exact matching

```c#
List<Product> products = Product.Get(new Product() {
	Name = "Product Name"
});
```


### Finding records using multiple search fields

```c#
List<Product> products = Product.Get(new Product() {
	Name = "Product Name",
	Category = 3
});
```

### Find records using fuzzy/TSQL-LIKE matching

```c#
List<Product> products = Product.Get(new Product() {
	Name = "%boring%"
}, true);
```

### Finding records using multiple all-inclusive (OR) terms

E.g. retrieve all products named "Boring Product Name" or "Uncle Bob".

```c#
List<Product> products = Product.Get(new List<DataObject>() {
	new Product() { Name = "Boring Product Name" },
	new Product() { Name = "Uncle Bob"}
});
```

## The sophisticated
Assume we have a long chain of related and properly mapped classes:

```c#
Content <-> Product <- LineItem -> Order <- Customer
```

Finding data related to a known entity or "distant" search term is effortless. QuickDAL will use the shortest mapped path between the entities.

### Finding records immediately related to a specific record

```c#
Order o = Order.Get(new Guid("{47E9A983-4655-4C34-BB6E-E2A4C315D428}"))
List<LineItem> lines = LineItem.Get(o);
```

### Finding records based on criteria from a related entity

```c#
Order searchOrder = new Order() { Date = DateTime.Today };
List<LineItem> linesOrderdToday = LineItem.Get(searchOrder);
```

### Finding "distant" records related to a specific record

E.g., find all Content available to a logged in customer.

```c#
Customer c = Customer.Get(GetLoggedInCustomerId());    // or something
List<Content> availableContent = Content.Get(c);
```

### Finding records based on "distant" search _criteria_.

E.g., find all Content available to Customers in Wisconsin.

```c#
Customer criteria = new Customer() { State = "WI" };
List<Content> avialableContent = Content.Get(criteria);
```

Well and good, aye!? You just need to learn how to get your entities mapped to the schema now.

## Get Started

* [Blessing an entity](docs/blessing.md)
* [Building Relationships](docs/relationships.md)
* [Injecting a database connection](docs/connections.md)
* Using your new [CRUD Methods](docs/crud.md)
