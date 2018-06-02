import { Injectable } from '@angular/core';

@Injectable()
export class ProtrConfigurationService {
  constructor() { }

  production = false;
  apiUrl = 'http:/server/api';
  loginUrl = '/Auth/Login';
  logoutUrl = '/Auth/Logout';

  get apiLogin() { return this.apiUrl + this.loginUrl; }

  get apiLogout() { return this.apiUrl + this.loginUrl; }
}
