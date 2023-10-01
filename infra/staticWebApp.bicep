param name string
param location string = 'eastasia'

param tags object = {}

param skuName string = 'Standard'
@secure()
param appInsightsId string
@secure()
param appInsightsInstrumentationKey string
@secure()
param appInsightsConnectionString string
@secure()
param facadeAppId string
param facadeLocation string

var staticApp = {
  name: 'sttapp-${name}'
  location: location
  tags: tags
  sku: {
    name: skuName
  }
  appInsights: {
    id: appInsightsId
    instrumentationKey: appInsightsInstrumentationKey
    connectionString: appInsightsConnectionString
  }
  linkedBackend: {
    id: facadeAppId
    location: facadeLocation
  }
}

resource sttapp 'Microsoft.Web/staticSites@2022-03-01' = {
  name: staticApp.name
  location: location
  tags: staticApp.tags
  sku: {
    name: staticApp.sku.name
  }
  properties: {
    allowConfigFileUpdates: true
    stagingEnvironmentPolicy: 'Enabled'
  }
}

resource sttappSettings 'Microsoft.Web/staticSites/config@2022-03-01' = {
  name: 'appsettings'
  parent: sttapp
  properties: {
    APPINSIGHTS_INSTRUMENTATIONKEY: staticApp.appInsights.instrumentationKey
    APPLICATIONINSIGHTS_CONNECTION_STRING: staticApp.appInsights.connectionString
  }
}

resource sttappLinkedBackend 'Microsoft.Web/staticSites/linkedBackends@2022-03-01' = {
  name: 'facade'
  parent: sttapp
  properties: {
    backendResourceId: staticApp.linkedBackend.id
    region: staticApp.linkedBackend.location
  }
}

output id string = sttapp.id
output name string = sttapp.name
output hostname string = sttapp.properties.defaultHostname
