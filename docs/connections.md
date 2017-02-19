# Database Connections

Each `Persistent` must provide a `GetQueryBuilder()` method that returns a `QueryBuilder`. Whether it returns an injected, service-located, or divinely inspired object is up to you. But, each `Persistent` entity must have one to tell QuickDAL where its data lives.

There are two ways to configure and build a `QueryBuilder`.

## Inject an [IDbConnection](http://msdn.microsoft.com/en-us/library/system.data.idbconnection(v=vs.110).aspx)

Just grab your connection from whatever connection manager you're using and give it to your `QueryBuilder`.

	IDbConnection c = MyConnectionManager.GetConnection();
	var qb = new QueryBuilder(c);

## Inject query execution functions

The alternative `QueryBuilder` constructor takes four proxy functions as parameters.
* `createCommand` : Returns an `IDbCommand`.
* `executeReader(IDbCommand)` : Executes an `IDbCommand` and returns an `IDataReader`.
* `executeNonQuery(IDbCommand)` : Executes an `IDbCommand` and returns an `Int32` (usually rows changed)
* `executeScalar(IDbCommand)` : Executes an `IDbComments` and returns an `Object` (usually the first column of the first row in the result set)

For example, if you're stuck using something like the Enterprise library (like me), you could create a working `QueryBuilder` like so:

	public static QueryBuilder GetQueryBuilder() {
	  Database db = DatabaseFactory.CreateDatabase("main");

	  Func<IDbCommand> createCommand = () =>
		{ return db.GetSqlStringCommand("select 1"); };

	  Func<IDbCommand, IDataReader> executeReader = (q) =>
		{ return db.ExecuteReader((DbCommand)q); };

	  Func<IDbCommand, Int32> executeNonQuery = (q) =>
		{ return db.ExecuteNonQuery((DbCommand)q); };

	  Func<IDbCommand, Object> executeScalar = (q) =>
		{ return db.ExecuteScalar((DbCommand)q); };

	  return new QueryBuilder(
		createCommand,
		executeReader,
		executeNonQuery,
		executeScalar
	  );
	}
