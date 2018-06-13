import { Injectable } from '@angular/core';
import { ProtrConfigurationService } from './configuration.service';
import { HttpClient } from '@angular/common/http';
import { ISimpleListRequest } from '../dtos/simpleListRequest';
import { ISimpleListResponseItem } from '../dtos/simpleListResponse';
import { AsyncCache, CacheDriver } from 'angular-async-cache';
import { Observable } from 'rxjs';

export class ProtrSimpleListService {
  constructor(protected httpClient: HttpClient,
    protected configurationService: ProtrConfigurationService,
    protected asyncCache: AsyncCache
  ) {
  }

  protected cacheDriver: CacheDriver;

  get(objectName: string,
      listName: string,
      parameter: string): Observable<ISimpleListResponseItem[]> {
    const simpleListRequest: ISimpleListRequest = {
      objectName, listName, parameter
    };
    const key = this.key(objectName, listName, parameter);
    const obs = this.httpClient
      .post<ISimpleListResponseItem[]>(this.configurationService.apiSimpleList, simpleListRequest);

    return this.asyncCache.wrap(obs, key, { } );
  }

  delete(objectName: string,
      listName: string,
      parameter: string): void {
    this.cacheDriver.delete(this.key(objectName, listName, parameter));
  }

  deleteList(objectName: string,
      listName: string): void {
    const prefix = this.key(objectName, listName, '');
    (<string[]>this.cacheDriver.keys())
      .filter(key => key.startsWith(prefix))
      .forEach(key => {
        this.cacheDriver.delete(key);
    });
  }

  key(objectName: string,
      listName: string,
      parameter: string): string {
    return 'SL_' + objectName + '_' + listName + '_' + parameter;
  }
}
