[![.NET](https://github.com/aimenux/OracleCli/actions/workflows/ci.yml/badge.svg?branch=main)](https://github.com/aimenux/OracleCli/actions/workflows/ci.yml)
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=OracleCli-Key&metric=coverage)](https://sonarcloud.io/summary/new_code?id=OracleCli-Key)
[![NuGet](https://img.shields.io/nuget/v/OracleCli)](https://www.nuget.org/packages/OracleCli/)

# OracleCli
```
A net global tool helping to retrieve some infos (packages, functions, procedures, etc.) from oracle
```

> In this repo, i m building a global tool that allows to retrieve schema(s), package(s), function(s), procedure(s) and parameter(s) infos from oracle.
>
> The tool is based on multiple sub commmands :
> - Use sub command `Info` or `Infos` to get oracle database infos
> - Use sub command `Table` or `Tables` to list tables from oracle
> - Use sub command `Schema` or `Schemas` to list schemas from oracle
> - Use sub command `Object` or `Objects` to list objects from oracle
> - Use sub command `Package` or `Packages` to list packages from oracle
> - Use sub command `Function` or `Functions` to list functions from oracle
> - Use sub command `Procedure` or `Procedures` to list procedures from oracle
> - Use sub command `Parameter` or `Parameters` to list procedure/function parameters from oracle
> - Use sub command `Source` or `Sources` to get procedure/function source code from oracle
> - Use sub command `Session` or `Sessions` to get active sessions from oracle
> - Use sub command `Lock` or `Locks` to get locked sessions from oracle

>
> To run the tool, type commands :
> - `OracleCli -h` to show help
> - `OracleCli -s` to show settings
> - `OracleCli Infos -d [db-name]` to get oracle database infos
> - `OracleCli Tables -d [db-name]` to list tables from oracle
> - `OracleCli Schemas -d [db-name]` to list schemas from oracle
> - `OracleCli Objects -d [db-name]` to list objects from oracle
> - `OracleCli Packages -d [db-name]` to list packages from oracle
> - `OracleCli Functions -d [db-name]` to list functions from oracle
> - `OracleCli Procedures -d [db-name]` to list procedures from oracle
> - `OracleCli Parameters -d [db-name] -p [pkg-name] -s [spc-name]` to list procedure parameters from oracle
> - `OracleCli Parameters -d [db-name] -p [pkg-name] -f [fun-name]` to list function parameters from oracle
> - `OracleCli Sources -d [db-name] -p [pkg-name] -s [spc-name]` to get procedure source code from oracle
> - `OracleCli Sources -d [db-name] -p [pkg-name] -f [fun-name]` to get function source code from oracle
> - `OracleCli Sessions -d [db-name]` to get active sessions from oracle
> - `OracleCli Locks -d [db-name]` to get locked sessions from oracle
>
>
> To install global tool from a local source path, type commands :
> - `dotnet tool install -g --configfile .\nugets\local.config OracleCli --version "*-*" --ignore-failed-sources`
>
> To install global tool from [nuget source](https://www.nuget.org/packages/OracleCli), type these command :
> - For stable version : `dotnet tool install -g OracleCli --ignore-failed-sources`
> - For prerelease version : `dotnet tool install -g OracleCli --version "*-*" --ignore-failed-sources`
>
> To uninstall global tool, type these command :
> - `dotnet tool uninstall -g OracleCli`
>
>

**`Tools`** : vs22, net 6.0/7.0, command-line, spectre-console, fluent-validation, dapper, xunit, test-containers, polly
