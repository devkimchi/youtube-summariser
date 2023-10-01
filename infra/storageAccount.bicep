param name string
param location string = resourceGroup().location

param tags object = {}

param storageContainerName string

var storage = {
  name: 'st${name}'
  location: location
  blobContainer: storageContainerName
  tags: tags
}

resource st 'Microsoft.Storage/storageAccounts@2022-09-01' = {
  name: storage.name
  location: storage.location
  kind: 'StorageV2'
  tags: storage.tags
  sku: {
    name: 'Standard_LRS'
  }
  properties: {
    supportsHttpsTrafficOnly: true
  }
}

resource stblob 'Microsoft.Storage/storageAccounts/blobServices@2022-09-01' = {
  name: 'default'
  parent: st
  properties: {
    deleteRetentionPolicy: {
      enabled: false
    }
    cors: {
      corsRules: [
        {
          allowedOrigins: [
            'https://make.powerapps.com'
            'https://make.powerautomate.com'
          ]
          allowedMethods: [
            'GET'
          ]
          allowedHeaders: [
            '*'
          ]
          exposedHeaders: [
            '*'
          ]
          maxAgeInSeconds: 0
        }
      ]
    }
  }
}

resource stblobcontainer 'Microsoft.Storage/storageAccounts/blobServices/containers@2022-09-01' = {
  name: storage.blobContainer
  parent: stblob
  properties: {
    immutableStorageWithVersioning: {
      enabled: false
    }
    defaultEncryptionScope: '$account-encryption-key'
    denyEncryptionScopeOverride: false
    publicAccess: 'Blob'
  }
}

output id string = st.id
output name string = st.name
output connectionString string = 'DefaultEndpointsProtocol=https;AccountName=${st.name};EndpointSuffix=${environment().suffixes.storage};AccountKey=${listKeys(st.id, '2022-09-01').keys[0].value}'
