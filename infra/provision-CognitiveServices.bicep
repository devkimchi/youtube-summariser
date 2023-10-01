param name string
param location string = 'eastus'

param tags object = {}

param aoaiModels array = []

module aoai './openAI.bicep' = {
  name: 'CognitiveServices_AOAI'
  params: {
    name: name
    location: location
    tags: tags
    aoaiModels: aoaiModels
  }
}

output name string = aoai.outputs.name
output endpoint string = aoai.outputs.endpoint
output apiKey string = aoai.outputs.apiKey
output deploymentId string = aoaiModels[0].deploymentName
