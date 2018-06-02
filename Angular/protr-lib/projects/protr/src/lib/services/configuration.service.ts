import { Injectable } from '@angular/core';

@Injectable()
export class ProtrConfigurationService {
  constructor() { }

  apiUrl = '/api';
  loginUrl = '/api/Auth/Login';
  logoutUrl = '/api/Auth/Logout';
}
