import { Environment } from '@abp/ng.core';

const baseUrl = 'http://localhost:4200';

const oAuthConfig = {
  issuer: 'http://localhost:44310/',
  redirectUri: baseUrl,
  clientId: 'ScriptedReviews_App',
  responseType: 'code',
  scope: 'offline_access ScriptedReviews',
  requireHttps: true,
};

export const environment = {
  production: true,
  application: {
    baseUrl,
    name: 'ScriptedReviews',
  },
  oAuthConfig,
  apis: {
    default: {
      url: 'http://localhost:44320',
      rootNamespace: 'ScriptedReviews',
    },
    AbpAccountPublic: {
      url: oAuthConfig.issuer,
      rootNamespace: 'AbpAccountPublic',
    },
  },
  remoteEnv: {
    url: '/getEnvConfig',
    mergeStrategy: 'deepmerge'
  }
} as Environment;
