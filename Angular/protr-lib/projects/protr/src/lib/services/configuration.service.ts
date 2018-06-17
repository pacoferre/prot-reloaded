import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { IDictionary } from '../interfaces/IDictionary';
import { IFieldProperty } from '../dtos/decorator';

export class ProtrConfigurationService {
  constructor(protected httpClient: HttpClient) { }

  production = false;
  apiUrl = 'http:/server/api';
  loginUrl = '/Authentication/Login';
  logoutUrl = '/Authentication/Logout';
  currentUserUrl = '/Authentication/CurrentUser';
  propertiesUrl = '/CRUD/Properties';
  crudPostUrl = '/CRUD/Post';
  simpleListUrl = '/CRUD/SimpleList';
  listDefinitionUrl = '/CRUD/ListDefinition';
  listUrl = '/CRUD/List';

  get apiLogin() { return this.apiUrl + this.loginUrl; }
  get apiLogout() { return this.apiUrl + this.logoutUrl; }
  get apiCurrentUser() { return this.apiUrl + this.currentUserUrl; }
  get apiProperties() { return this.apiUrl + this.propertiesUrl; }
  get apiCrudPost() { return this.apiUrl + this.crudPostUrl; }
  get apiSimpleList() { return this.apiUrl + this.simpleListUrl; }
  get apiListDefinition() { return this.apiUrl + this.listDefinitionUrl; }
  get apiList() { return this.apiUrl + this.listUrl; }

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
    if (environment.simpleListUrl) {
      this.simpleListUrl = environment.simpleListUrl;
    }
    if (environment.listDefinitionUrl) {
      this.listDefinitionUrl = environment.listDefinitionUrl;
    }
    if (environment.listUrl) {
      this.listUrl = environment.listUrl;
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
