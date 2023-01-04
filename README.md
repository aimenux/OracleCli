[![.NET](https://github.com/aimenux/OracleCli/actions/workflows/ci.yml/badge.svg?branch=main)](https://github.com/aimenux/OracleCli/actions/workflows/ci.yml)

# OracleCli
```
A net global tool helping to retrieve package(s), function(s), procedure(s) and argument(s) infos from oracle
```

> In this repo, i m building a global tool that allows to retrieve package(s), function(s), procedure(s) and argument(s) infos from oracle.
>
> The tool is based on multiple sub commmands :
> - Use sub command `Package` to list packages from oracle
> - Use sub command `Function` to list functions from oracle
> - Use sub command `Procedure` to list procedures from oracle
> - Use sub command `Argument` to list procedure arguments from oracle

>
> To run the tool, type commands :
> - `Oracle -h` to show help
> - `Oracle -s` to show settings
> - `Oracle Package -d [db-name]` to list packages from oracle
> - `Oracle Function -d [db-name]` to list functions from oracle
> - `Oracle Procedure -d [db-name]` to list procedures from oracle
> - `Oracle Argument -d [db-name] -p [pkg-name] -s [spc-name]` to list procedure arguments from oracle
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

**`Tools`** : vs22, net 7.0, command-line, spectre-console, dapper, xunit, fluent-assertions, test-containers
