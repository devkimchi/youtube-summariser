param name string
param suffix string
param location string = resourceGroup().location

param tags object = {}

@secure()
param storageAccountConnectionString string

@secure()
param appInsightsInstrumentationKey string
@secure()
param appInsightsConnectionString string

param consumptionPlanId string

param openApiSettings array = [
  {
    name: ''
    value: ''
  }
]
param aoaiServiceSettings array = [
  {
    name: ''
    value: ''
  }
]
param promptSettings array = [
  {
    name: ''
    value: ''
  }
]
param apimSettings array = [
  {
    name: ''
    value: ''
  }
]
param graphSettings array = [
  {
    name: ''
    value: ''
  }
]

var storage = {
  connectionString: storageAccountConnectionString
}

var appInsights = {
  instrumentationKey: appInsightsInstrumentationKey
  connectionString: appInsightsConnectionString
}

var consumption = {
  id: consumptionPlanId
}

var functionApp = {
  name: 'fncapp-${name}-${suffix}'
  location: location
  tags: tags
}

var commonSettings = [
  {
    name: 'APPINSIGHTS_INSTRUMENTATIONKEY'
    value: appInsights.instrumentationKey
  }
  {
    name: 'APPLICATIONINSIGHTS_CONNECTION_STRING'
    value: appInsights.connectionString
  }
  {
    name: 'AzureWebJobsStorage'
    value: storage.connectionString
  }
  {
    name: 'FUNCTION_APP_EDIT_MODE'
    value: 'readonly'
  }
  {
    name: 'FUNCTIONS_EXTENSION_VERSION'
    value: '~4'
  }
  {
    name: 'FUNCTIONS_WORKER_RUNTIME'
    value: 'dotnet-isolated'
  }
  {
    name: 'WEBSITE_CONTENTAZUREFILECONNECTIONSTRING'
    value: storage.connectionString
  }
  {
    name: 'WEBSITE_CONTENTSHARE'
    value: functionApp.name
  }
]

var appSettings = concat(commonSettings, openApiSettings, aoaiServiceSettings, promptSettings, apimSettings, graphSettings)

resource fncapp 'Microsoft.Web/sites@2022-03-01' = {
  name: functionApp.name
  location: functionApp.location
  kind: 'functionapp'
  tags: functionApp.tags
  properties: {
    serverFarmId: consumption.id
    httpsOnly: true
    siteConfig: {
      appSettings: appSettings
    }
  }
}

var policies = [
  {
    name: 'scm'
    allow: false
  }
  {
    name: 'ftp'
    allow: false
  }
]

resource fncappPolicies 'Microsoft.Web/sites/basicPublishingCredentialsPolicies@2022-03-01' = [for policy in policies: {
  name: policy.name
  parent: fncapp
  properties: {
    allow: policy.allow
  }
}]

output id string = fncapp.id
output name string = fncapp.name
