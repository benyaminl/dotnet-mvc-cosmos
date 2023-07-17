# Contact Azure Cosmos NoSQL

This is a playground for azure cosmos using system.text.json rather than newton, can run on free instance of app service in azure

This only a playground, don't expect much

## Config 

The config is in appsettings.json, and `COSMOS_ENDPOINT` also `COSMOS_KEY` should be configured. 

## Testing and Code Coverage 

For do test, we can run 

```
dotnet test --collect:"Code Coverage" /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura
```

To covert the xml to HTML we can run 

```
reportgenerator -reports:"coverage.cobertura.xml" -targetdir:"coveragereport" -reporttypes:Html
```

Then on Linux, XFCE, we can open the file while running (specific to XFCE Distro... I'm using Fedora XFCE Spins)

```
exo-open coveragereport/index.html
```