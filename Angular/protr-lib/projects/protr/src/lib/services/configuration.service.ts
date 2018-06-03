import { Injectable } from '@angular/core';

@Injectable()
export class ProtrConfigurationService {
  constructor() { }

  production = false;
  apiUrl = 'http:/server/api';
  loginUrl = '/Authentication/Login';
  logoutUrl = '/Authentication/Logout';
  currentUserUrl = '/Authentication/CurrentUser';

  get apiLogin() { return this.apiUrl + this.loginUrl; }

  get apiLogout() { return this.apiUrl + this.logoutUrl; }

  get apiCurrentUser() { return this.apiUrl + this.currentUserUrl; }
}
