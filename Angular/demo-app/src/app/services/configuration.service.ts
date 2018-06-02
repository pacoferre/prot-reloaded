import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { ProtrConfigurationService } from 'protr';

@Injectable()
export class ConfigurationService extends ProtrConfigurationService {
  constructor() {
    super();

    this.production = environment.production;
    this.apiUrl = environment.apiUrl;
    this.loginUrl = environment.loginUrl;
    this.logoutUrl = environment.logoutUrl;
   }
}
