# YouTube Summariser

This provides sample Blazor apps that summarise a YouTube video transcript to a given language.

## Prerequisites

- [.NET 7 SDK](https://dotnet.microsoft.com/download/dotnet/7.0?WT.mc_id=dotnet-96932-juyoo)
- [Visual Studio](https://visualstudio.microsoft.com/vs?WT.mc_id=dotnet-96932-juyoo) or [Visual Studio Code](https://code.visualstudio.com?WT.mc_id=dotnet-96932-juyoo) with [C# Dev Kit](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csdevkit&WT.mc_id=dotnet-96932-juyoo)
- [Azure Subscription](https://azure.microsoft.com/free?WT.mc_id=dotnet-96932-juyoo)
- [Azure OpenAI Service](https://learn.microsoft.com/azure/ai-services/openai/overview?WT.mc_id=dotnet-96932-juyoo)

## Getting Started

### Provision Azure OpenAI Service

1. Fork this repository to your GitHub account, `{{GITHUB_USERNAME}}`.
1. Run the commands below to set up a resource names:

   ```bash
   # PowerShell
   $AZURE_ENV_NAME="assistant$(Get-Random -Min 1000 -Max 9999)"
   $GITHUB_USERNAME="{{GITHUB_USERNAME}}"

   # Bash
   AZURE_ENV_NAME="assistant$RANDOM"
   GITHUB_USERNAME="{{GITHUB_USERNAME}}"
   ```

1. Run the commands below to provision Azure resources:

   ```bash
   azd auth login
   azd init -e $AZURE_ENV_NAME
   azd up
   ```

1. Get the endpoint, API key and deployment ID.

   ```bash
   # PowerShell
   $AOAI_ENDPOINT_URL = $(az cognitiveservices account show `
                            -g rg-$AZURE_ENV_NAME `
                            -n aoai-$AZURE_ENV_NAME `
                            --query "properties.endpoint" -o tsv)

   $AOAI_API_KEY = $(az cognitiveservices account keys list `
                       -g rg-$AZURE_ENV_NAME `
                       -n aoai-$AZURE_ENV_NAME `
                       --query "key1" -o tsv)

   $AOAI_DEPLOYMENT_ID = $(az cognitiveservices account deployment list `
                             -g rg-$AZURE_ENV_NAME `
                             -n aoai-$AZURE_ENV_NAME `
                             --query "[0].name" -o tsv)

   # Bash
   AOAI_ENDPOINT_URL=$(az cognitiveservices account show \
                         -g rg-$AZURE_ENV_NAME \
                         -n aoai-$AZURE_ENV_NAME \
                         --query "properties.endpoint" -o tsv)

   AOAI_API_KEY=$(az cognitiveservices account keys list \
                    -g rg-$AZURE_ENV_NAME \
                    -n aoai-$AZURE_ENV_NAME \
                    --query "key1" -o tsv)

   AOAI_DEPLOYMENT_ID=$(az cognitiveservices account deployment list \
                          -g rg-$AZURE_ENV_NAME \
                          -n aoai-$AZURE_ENV_NAME \
                          --query "[0].name" -o tsv)
   ```

### Deprovision Azure OpenAI Service

1. To avoid unexpected billing shock, run the commands below to deprovision Azure resources:

   ```bash
   azd down --force --purge --no-prompt
   ```

### Power Platform Custom Connector in Visual Studio

TBD

### Teams App with Blazor Server App in Visual Studio

TBD

### Blazor WebAssembly App in Visual Studio Code

1. Copy `local.settings.sample.json` to `local.settings.json`
1. Update `local.settings.json` file for backend API.

   ```json
   "OpenAI__Endpoint": "{{AOAI_ENDPOINT_URL}}",
   "OpenAI__ApiKey": "{{AOAI_API_KEY}}",
   "OpenAI__DeploymentId": "{{AOAI_DEPLOYMENT_ID}}",
   ```

1. Build the entire solution.

   ```bash
   dotnet restore && dotnet build
   ```

1. Open a new terminal and run the backend API app.

   ```bash
   pushd src/YouTubeSummariser.ApiApp
   func start
   popd
   ```

1. Open a new terminal and run the Blazor WebAssembly app.

   ```bash
   pushd /src/YouTubeSummariser.WebApp.Wasm
   dotnet watch run
   popd
   ```
