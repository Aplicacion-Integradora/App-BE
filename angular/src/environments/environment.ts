import { Environment } from '@abp/ng.core';

const baseUrl = 'http://localhost:4200';

const oAuthConfig = {
  issuer: 'https://localhost:44322/',
  redirectUri: baseUrl,
  clientId: 'ScriptedReviews_App',
  responseType: 'code',
  scope: 'offline_access ScriptedReviews',
  requireHttps: true,
};

export const environment = {
  production: false,
  application: {
    baseUrl,
    name: 'ScriptedReviews',
  },
  oAuthConfig,
  apis: {
    default: {
      url: 'https://localhost:44322',
      rootNamespace: 'ScriptedReviews',
    },
    AbpAccountPublic: {
      url: oAuthConfig.issuer,
      rootNamespace: 'AbpAccountPublic',
    },
  },
} as Environment;
