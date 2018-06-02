import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { ProtrConfigurationService } from 'protr';

@Injectable()
export class ConfigurationService extends ProtrConfigurationService {
  constructor() {
    super();

    this.apiUrl = '/api';
    this.loginUrl = '/api/Auth/Login';
    this.logoutUrl = '/api/Auth/Logout';
   }
}
