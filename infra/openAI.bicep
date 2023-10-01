param name string
param location string = 'eastus'

param tags object = {}

param aoaiModels array = []

var openai = {
    name: 'aoai-${name}'
    location: location
    tags: tags
    skuName: 'S0'
    models: aoaiModels
}

resource aoai 'Microsoft.CognitiveServices/accounts@2023-05-01' = {
    name: openai.name
    location: openai.location
    kind: 'OpenAI'
    tags: openai.tags
    sku: {
        name: openai.skuName
    }
    properties: {
        customSubDomainName: openai.name
        publicNetworkAccess: 'Enabled'
    }
}

resource aoaiDeployment 'Microsoft.CognitiveServices/accounts/deployments@2023-05-01' = [for model in openai.models: {
  name: model.deploymentName
  parent: aoai
  sku: {
    name: model.skuName
    capacity: model.skuCapacity
  }
  properties: {
    model: {
      format: 'OpenAI'
      name: model.name
      version: model.version
    }
  }
}]

output id string = aoai.id
output name string = aoai.name
output endpoint string = aoai.properties.endpoint
output apiKey string = listKeys(aoai.id, '2023-05-01').key1
