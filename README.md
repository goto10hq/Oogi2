![Oogi2](https://github.com/goto10hq/Oogi2/raw/master/oogi2-icon.png)

# Oogi2

[![Software License](https://img.shields.io/badge/license-MIT-brightgreen.svg?style=flat-square)](LICENSE.md)
[![Latest Version on NuGet](https://img.shields.io/nuget/v/Oogi2.svg?style=flat-square)](https://www.nuget.org/packages/Oogi2/)
[![NuGet](https://img.shields.io/nuget/dt/Oogi2.svg?style=flat-square)](https://www.nuget.org/packages/Oogi2/)
[![Visual Studio Team services](https://img.shields.io/vso/build/frohikey/c3964e53-4bf3-417a-a96e-661031ef862f/118.svg?style=flat-square)](https://github.com/goto10hq/Oogi2)

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

TODO: finish

## TODO

- test dynamic objects (CommonRepository) without Id set

## License

MIT © [frohikey](http://frohikey.com) / [Goto10 s.r.o.](http://www.goto10.cz)
