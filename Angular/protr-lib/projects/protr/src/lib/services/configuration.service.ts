import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { IDictionary } from '../interfaces/IDictionary';
import { IFieldProperty } from '../dtos/decorator';

@Injectable()
export class ProtrConfigurationService {
  constructor(protected httpClient: HttpClient) { }

  production = false;
  apiUrl = 'http:/server/api';
  loginUrl = '/Authentication/Login';
  logoutUrl = '/Authentication/Logout';
  currentUserUrl = '/Authentication/CurrentUser';
  propertiesUrl = '/CRUD/Properties';
  crudPostUrl = '/CRUD/Post';

  get apiLogin() { return this.apiUrl + this.loginUrl; }

  get apiLogout() { return this.apiUrl + this.logoutUrl; }

  get apiCurrentUser() { return this.apiUrl + this.currentUserUrl; }

  get apiProperties() { return this.apiUrl + this.propertiesUrl; }

  get apiCrudPost() { return this.apiUrl + this.crudPostUrl; }

  loadFromEnvironment(environment: any) {
    this.production = environment.production;
    this.apiUrl = environment.apiUrl;
    if (environment.loginUrl) {
      this.loginUrl = environment.loginUrl;
    }
    if (environment.logoutUrl) {
      this.logoutUrl = environment.logoutUrl;
    }
    if (environment.currentUserUrl) {
      this.currentUserUrl = environment.currentUserUrl;
    }
    if (environment.propertiesUrl) {
      this.propertiesUrl = environment.propertiesUrl;
    }
    if (environment.crudPostUrl) {
      this.crudPostUrl = environment.crudPostUrl;
    }
  }

  getProperties(name: string): Promise<IDictionary<IFieldProperty>> {
    return new Promise<any>(resolve => {
      const options = { params: new HttpParams().set('name', name) };

      this.httpClient
        .get<IDictionary<IFieldProperty>>(this.apiProperties, options)
        .subscribe(resp => {
          resolve(resp);
        }, error => {
          resolve(null);
        });
    });
  }
}
