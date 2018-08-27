![Oogi2](https://github.com/goto10hq/Oogi2/raw/master/oogi2-icon.png)

# Oogi2

[![Software License](https://img.shields.io/badge/license-MIT-brightgreen.svg?style=flat-square)](LICENSE.md)
[![Latest Version on NuGet](https://img.shields.io/nuget/v/Oogi2.svg?style=flat-square)](https://www.nuget.org/packages/Oogi2/)
[![NuGet](https://img.shields.io/nuget/dt/Oogi2.svg?style=flat-square)](https://www.nuget.org/packages/Oogi2/)
[![Visual Studio Team services](https://img.shields.io/vso/build/frohikey/c3964e53-4bf3-417a-a96e-661031ef862f/118.svg?style=flat-square)](https://github.com/goto10hq/Oogi2)
[![.NETStandard 2.0](https://img.shields.io/badge/.NETStandard-2.0-blue.svg?style=flat-square)](https://github.com/dotnet/standard/blob/master/docs/versions/netstandard2.0.md)

## Connection

Connection is basic configuration object. Just initialize it simply like that:

``var connection = new Connection(endpoint, authorizationKey, database, collection);``

Some things keep in mind:
- if Windows OS is detected, connection mode ``Direct`` and connection protocol ``Tcp`` is being used, otherwise we ended up with ``Gateway`` mode since ``Direct`` one doesn't work on OSX/Linux
- default JSON ser/deser settings are being set:

```csharp
Formatting = Formatting.None,
TypeNameHandling = TypeNameHandling.None,
ContractResolver = new CamelCasePropertyNamesContractResolver()
```
- if you don't specify ``ConnectionPolicy`` some ultra safe retry policy settings are being used

```csharp
MaxRetryAttemptsOnThrottledRequests = 1000,
MaxRetryWaitTimeInSeconds = 60
```

### Methods

``DocumentCollection CreateCollection()``

Create collection if it doesn't exists.

``bool DeleteCollection()``

Delete collection.

## Repository

Generic and non-generic repositories are available. 

Easiest way is using non-generic repo like that:

```csharp
var repo = new Repository(connection);

repo.Create(new { Movie = "Donkey Kong Jr.", Rating = 3 });
repo.Create(new { Movie = "King Kong", Rating = 2 });
repo.Create(new { Movie = "Donkey Kong", Rating = 1 });

var games = repo.GetList("select * from c where c.rating = @rating", new { rating = 3 });
```

In most cases you'd like to use POCOs like that:

```csharp
const string _entity = "robot";

[EntityType("entity", _entity)]
public class Robot
{
 public string Id { get; set; }            
 public string Name { get; set; }
 public int ArtificialIq { get; set; }
 public Stamp Created { get; set; } = new Stamp();
 public bool IsOperational { get; set; }
}
``` 
We have a special attribute ``[Entity]`` useful for setting document type property. It's optional of course. If you use this attribute condition is automatically injected in "where" phrase in SQL.

```csharp
var repo = new Repository<Robot>(connection);
var robot = repo.GetFirstOrDefault(); // generated sql "select top 1 * from c where c.entity = 'robot'
var robots = repo.GetList("select * from c where c.entity = @entity and c.artificialIq > @iq",
new
{
    entity = _entity,
    iq = 120
});
```

### Methods for repository

``T GetFirstOrDefault(SqlQuerySpec query, FeedOptions feedOptions = null)``

``T GetFirstOrDefault(DynamicQuery query, FeedOptions feedOptions = null)``

``T GetFirstOrDefault(string sql, object parameters, FeedOptions feedOptions = null)``

``T GetFirstOrDefault(string id, FeedOptions feedOptions = null)``

Get first document found or null. Three types of queries, direct Id of the document or just ``null`` _(in this case EntityType is auto included)_.

``T Upsert(T entity, RequestOptions requestOptions = null)``

Upsert document.

``T Create(T entity, RequestOptions requestOptions = null)``

Create document.

``T Replace(T entity, RequestOptions requestOptions = null)``

Replace document.

``bool Delete(string id, RequestOptions requestOptions = null)``

``bool Delete(T entity, RequestOptions requestOptions = null)``

Delete document.

``IList<T> GetAll(FeedOption feedOptions = null)``

Get all documents _(in this case EntityType is auto included)_.

``IList<T> GetList(SqlQuerySpec query, FeedOptions feedOptions = null)``

``IList<T> GetList(DynamicQuery query, FeedOptions feedOptions = null)``

``IList<T> GetList(string query, object parameters = null, FeedOptions feedOptions = null)``

Get list of documents.

## Aggregate repository

Helper repository for aggregate calls. It automagically handles possibility of partial results.

```csharp
var agr = new AggregateRepository(connection);
var einstein = repo.GetList("select max(c.iq) from c where c.gender", new { gender = "M" });
```

### Methods for aggregate repository

``long? Get(SqlQuerySpec query, FeedOptions feedOptions = null)``

``long? Get(DynamicQuery query, FeedOptions feedOptions = null)``

``long? Get(string sql, object parameters, FeedOptions feedOptions = null)``

Get a numeric result of aggregate function.

## Tokens

Since it's kinda complicated to filter stamps (datetimes) directly through SQL queries. We have two timestamp objects:

### SimpleStamp

``var created = new SimpleStamp(dateTime);``

In JSON:

```json
"created": {
 "dateTime": "2017-03-29T15:58:50.8828571+02:00",
 "epoch": 1490803130
}
```

### Stamp

``var created = new Stamp(dateTime);``

In JSON:

```json
"created": {
 "dateTime": "2016-09-13T20:06:22.3214018+02:00",
 "epoch": 1473797182,
 "year": 2016,
 "month": 9,
 "day": 13,
 "hour": 20,
 "minute": 6,
 "second": 22
}
```

Stamps are automatically compared by ``epoch``:

```csharp
var q = new DynamicQuery("select * from c where c.epoch = @stamp or c.epoch2 = @stamp2", 
new
{
    stamp = new Stamp(new DateTime(2000, 1, 1)),
    stamp2 = new SimpleStamp(new DateTime(2001, 1, 1))
});
```

results in:

``select * from c where c.epoch = 946684800 or c.epoch2 = 9466848002``
            
## Queries

### DynamicQuery

More conformatable way:

```csharp
var ids = new List<int> { 4, 5, 2 };
var q = new DynamicQuery("select * from c where c.items in @ids", new { ids });            
```

results in:

``select * from c where c.items in (4,5,2)``

### SqlQuerySpec

var q = new SqlQuerySpec
(
    "select * from c where c.entity = @entity",
    new SqlParameterCollection
    {
        new SqlParameter("@entity", "robot"),
    }
);

results in:

``select * from c where c.entity = 'robot'``

### PureString

Of course.

### Linq

Not at the moment. I don't like it very much. I'm gonna implement it maybe in the future.

## License

MIT © [frohikey](http://frohikey.com) / [Goto10 s.r.o.](http://www.goto10.cz)
