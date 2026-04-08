# PostgreSQL Provider for Umbraco


[![Downloads](https://img.shields.io/nuget/dt/Our.Umbraco.PostgreSql?color=cc9900)](https://www.nuget.org/packages/Our.Umbraco.PostgreSql/)
[![NuGet](https://img.shields.io/nuget/vpre/Our.Umbraco.PostgreSql?color=0273B3)](https://www.nuget.org/packages/Our.Umbraco.PostgreSql)
[![GitHub license](https://img.shields.io/github/license/idseefeld/PostgreSqlForUmbraco?color=8AB803)](../LICENSE)


```
ATTENTION: This is a community driven project and is not officially supported by Umbraco HQ.

All Umbraco Unit and Integration test are passing successfully.

I did my best to implement a PostgreSQL provider for Umbraco, but there might be some edge cases that I haven't covered yet. 
If you find any issues or have suggestions for improvements, please don't hesitate to reach out.

```


### Comments and contributions are very welcome!

If you are curios about the progress, please have a look into my fork of Umbraco on GitHub:

[Our.Umbraco.PostgreSql](https://github.com/idseefeld/Umbraco-CMS/tree/v173/postgreSqlProvider)



## Install PostgreSQL Database
1. Download and install PostgreSQL (version 16, 17 or 18) from the [official website](https://www.postgresql.org/download/) or especially for [Windwos](https://www.postgresql.org/download/windows/).
1. Create a new database for Umbraco using the PostgreSQL command line or a GUI tool like pgAdmin. Follow my [tutorial video](https://youtu.be/6ruTSbTdzSk).
1. Start debugging this solution [F5]
1. If not trusting databse certificate "SSL Mode" is set to `VerifyCA` during installation. But you can change this later on in the connection string in `appsettings.json` file. Read details: https://www.npgsql.org/doc/security.html?tabs=tabid-1

## Documentation and Issue Tracker

You can find more details in the [Documentation](https://github.com/idseefeld/PostgreSqlForUmbraco/wiki).
If you find any issues or want to ask for features, please use the [IssueTracker](https://github.com/idseefeld/PostgreSqlForUmbraco/issues)
