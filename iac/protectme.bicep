extension microsoftGraphV1_0

resource clientApp 'Microsoft.Graph/applications@v1.0' = {
  uniqueName: 'DesktopClientApp'
  displayName: 'DesktopClientApp'
  publicClient: {
    redirectUris: [
      'https://localhost'
    ]
  }
  requiredResourceAccess: [
    {
      resourceAppId: app.appId
      resourceAccess: [
        {
          id: app.api.oauth2PermissionScopes[0].id
          type: 'Scope'
        }
      ]
    }
  ]
    
}

resource app 'Microsoft.Graph/applications@v1.0' = {
  uniqueName: 'NewWebApi1'
  displayName: 'NewWebApi1'
  signInAudience: 'AzureADMyOrg'    
  publicClient: {
    redirectUris: [
      'http://localhost'
    ]
  }
  identifierUris: [
    'api://NewWebApi1'
  ]
  api : {    
    oauth2PermissionScopes: [
      { 
        adminConsentDescription: 'Allows the Application to read weather forecast data'
        adminConsentDisplayName: 'Read Forecast Data'
        userConsentDescription: 'Allows the User to read weather forecast data'
        userConsentDisplayName: 'Read Forecast Data'
        id: 'bced9889-2916-4c9e-a043-e8bdf7e4ff37'
        isEnabled: true
        type: 'User'
        value: 'Forecast.Read'
      }
    ]    
  }
  tags: [
    'WindowsAzureActiveDirectoryIntegratedApp'
  ]
  appRoles: [
    {
      allowedMemberTypes: [
        'User'
        'Application'
      ]
      description: 'Read all users'
      displayName: 'Read all users'      
      id: 'a1b2c3d4-e5f6-7890-abcd-ef1234567890'
      isEnabled: true
      value: 'NewWebApi1.Read'
    }
    {
      allowedMemberTypes: [
        'User'
        'Application'
      ]
      description: 'Admin Functions'
      displayName: 'Admin Functions'
      id: 'f8d3b9e3-4d9c-4d3b-8f3e-5e6c2b6aa0f6'
      isEnabled: true
      value: 'NewWebApi1.Admin'
    }
  ]
}


resource spn 'Microsoft.Graph/servicePrincipals@v1.0' = {
  displayName: 'NewWebApi1'
  servicePrincipalType: 'Application'
  appId: app.appId
  tags: [
    'WindowsAzureActiveDirectoryIntegratedApp'
  ]
}
