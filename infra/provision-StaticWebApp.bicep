param name string
param location string = resourceGroup().location

param tags object = {}

module wrkspc './logAnalyticsWorkspace.bicep' = {
  name: 'StaticWebApp_LogAnalyticsWorkspace'
  params: {
    name: '${name}-web'
    location: location
    tags: tags
  }
}

module appins 'applicationInsights.bicep' = {
  name: 'StaticWebApp_ApplicationInsights'
  params: {
    name: '${name}-web'
    location: location
    workspaceId: wrkspc.outputs.id
    tags: tags
  }
}

resource fncapp 'Microsoft.Web/sites@2022-03-01' existing = {
  name: 'fncapp-${name}-facade'
}

module sttapp './staticWebApp.bicep' = {
  name: 'StaticWebApp_StaticWebApp'
  params: {
    name: '${name}-web'
    location: location
    tags: tags
    appInsightsId: appins.outputs.id
    appInsightsInstrumentationKey: appins.outputs.instrumentationKey
    appInsightsConnectionString: appins.outputs.connectionString
    facadeAppId: fncapp.id
    facadeLocation: fncapp.location
  }
}

output id string = sttapp.outputs.id
output name string = sttapp.outputs.name
output hostname string = sttapp.outputs.hostname
