import { Injectable } from '@angular/core';
import { ProtrConfigurationService } from './configuration.service';
import { HttpClient } from '@angular/common/http';
import { AsyncCache, CacheDriver } from 'angular-async-cache';
import { Observable } from 'rxjs';
import { IListDefinitionRequest } from '../dtos/listDefinitionRequest';

export class ProtrFilteringService {
  constructor(protected httpClient: HttpClient,
    protected configurationService: ProtrConfigurationService,
    protected asyncCache: AsyncCache
  ) {
  }
  protected cacheDriver: CacheDriver;

  get(objectName: string,
      filterName: string): Observable<any[]> {
    const simpleListRequest: IListDefinitionRequest = {
      objectName, filterName
    };
    const key = this.key(objectName, filterName);
    const obs = this.httpClient
      .post<any[]>(this.configurationService.apiCrudPost, simpleListRequest);

    return this.asyncCache.wrap(obs, key, { } );
  }

  delete(objectName: string,
    filterName: string): void {
  this.cacheDriver.delete(this.key(objectName, filterName));
}

  key(objectName: string,
      filterName: string): string {
    return 'FL_' + objectName + '_' + filterName;
  }
}
