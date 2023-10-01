param name string
param location string = resourceGroup().location

param tags object = {}

param apiManagementPublisherName string
param apiManagementPublisherEmail string

@allowed([
  'rawxml'
  'rawxml-link'
  'xml'
  'xml-link'
])
param apiManagementPolicyFormat string = 'xml'
param apiManagementPolicyValue string = '<!--\r\n  IMPORTANT:\r\n  - Policy elements can appear only within the <inbound>, <outbound>, <backend> section elements.\r\n  - Only the <forward-request> policy element can appear within the <backend> section element.\r\n  - To apply a policy to the incoming request (before it is forwarded to the backend service), place a corresponding policy element within the <inbound> section element.\r\n  - To apply a policy to the outgoing response (before it is sent back to the caller), place a corresponding policy element within the <outbound> section element.\r\n  - To add a policy position the cursor at the desired insertion point and click on the round button associated with the policy.\r\n  - To remove a policy, delete the corresponding policy statement from the policy document.\r\n  - Policies are applied in the order of their appearance, from the top down.\r\n-->\r\n<policies>\r\n  <inbound />\r\n  <backend>\r\n    <forward-request />\r\n  </backend>\r\n  <outbound />\r\n  <on-error />\r\n</policies>'

module wrkspc './logAnalyticsWorkspace.bicep' = {
  name: 'APIManagement_LogAnalyticsWorkspace'
  params: {
    name: name
    location: location
    tags: tags
  }
}

module appins './applicationInsights.bicep' = {
  name: 'APIManagement_ApplicationInsights'
  params: {
    name: name
    location: location
    tags: tags
    workspaceId: wrkspc.outputs.id
  }
}

module apim './apiManagement.bicep' = {
  name: 'ApiManagement_ApiManagement'
  params: {
    name: name
    location: location
    tags: tags
    appInsightsId: appins.outputs.id
    appInsightsInstrumentationKey: appins.outputs.instrumentationKey
    apiManagementPublisherName: apiManagementPublisherName
    apiManagementPublisherEmail: apiManagementPublisherEmail
    apiManagementPolicyFormat: apiManagementPolicyFormat
    apiManagementPolicyValue: apiManagementPolicyValue
  }
}

output id string = apim.outputs.id
output name string = apim.outputs.name
output subscriptionKey string = apim.outputs.subscriptionKey
