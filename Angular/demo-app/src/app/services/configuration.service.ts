import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { ProtrConfigurationService } from 'protr';

@Injectable()
export class ConfigurationService extends ProtrConfigurationService {
  constructor() {
    super();

    this.production = environment.production;
    this.apiUrl = environment.apiUrl;
    if ((<any>environment).loginUrl) {
      this.loginUrl = (<any>environment).loginUrl;
    }
    if ((<any>environment).logoutUrl) {
      this.logoutUrl = (<any>environment).logoutUrl;
    }
    if ((<any>environment).currentUserUrl) {
      this.currentUserUrl = (<any>environment).currentUserUrl;
    }
  }
}
