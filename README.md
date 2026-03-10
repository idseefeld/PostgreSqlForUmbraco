# PostgreSqlForUmbraco

[![Downloads](https://img.shields.io/nuget/dt/Our.Umbraco.PostgreSql?color=cc9900)](https://www.nuget.org/packages/Our.Umbraco.PostgreSql/)
[![NuGet](https://img.shields.io/nuget/vpre/Our.Umbraco.PostgreSql?color=0273B3)](https://www.nuget.org/packages/Our.Umbraco.PostgreSql)
[![GitHub license](https://img.shields.io/github/license/idseefeld/PostgreSqlForUmbraco?color=8AB803)](../LICENSE)

### Documentation for the PostgreSQL Provider Package for Umbraco CMS

```
ATTENTION: This is a community driven project and is not officially supported by Umbraco HQ.

Currently it is still in early development and should be used for testing and evaluation 
purposes only. And there are several issues in Umbraco's core that prevent full functionality 
when using PostgreSQL as the database. These are already addressed and will hopefully be 
fixed in future releases of Umbraco.
```

### Comments and contributions are very welcome!

Please report any issue to the [Issue Tracker](/idseefeld/PostgreSqlForUmbraco.issues.git).


### Install PostgreSQL Database
1. Download and install PostgreSQL (version 16, 17 or 18) from the [official website](https://www.postgresql.org/download/) or especially for [Windwos](https://www.postgresql.org/download/windows/).
1. Create a new database for Umbraco using the PostgreSQL command line or a GUI tool like pgAdmin. Follow my [tutorial video](https://youtu.be/6ruTSbTdzSk).
1. Start debugging this solution [F5]
1. If not trusting databse certificate "SSL Mode" is set to `VerifyCA` during installation. But you can change this later on in the connection string in `appsettings.json` file. Read details: https://www.npgsql.org/doc/security.html?tabs=tabid-1

### Wiki
Further details are in the [Wiki](https://github.com/idseefeld/PostgreSqlForUmbraco.wiki.git) of this project.
