import { Injectable } from '@angular/core';
import { ProtrConfigurationService } from './configuration.service';
import { HttpClient } from '@angular/common/http';
import { AsyncCache, CacheDriver } from 'angular-async-cache';
import { Observable, BehaviorSubject } from 'rxjs';
import { IListDefinitionRequest } from '../dtos/listDefinitionRequest';
import { IListDefinitionColumn } from '../dtos/listDefinitionColumn';
import { IListResponse } from '../dtos/listResponse';
import { ListRequest } from '../dtos/listRequest';
import { IDictionary } from 'protr/protr';

export class ProtrFilteringService {
  protected cacheDriver: CacheDriver;
  private _resultsSubject: IDictionary<BehaviorSubject<IListResponse>> = {};
  private resultsObservable: IDictionary<Observable<IListResponse>> = {};
  private _requestsSubject: IDictionary<BehaviorSubject<ListRequest>> = {};
  private requestsObservable: IDictionary<Observable<ListRequest>> = {};
  private _busySubject: IDictionary<BehaviorSubject<boolean>> = {};
  private busyObservable: IDictionary<Observable<boolean>> = {};

  constructor(protected httpClient: HttpClient,
    protected configurationService: ProtrConfigurationService,
    protected asyncCache: AsyncCache
  ) {
  }

  getListDefinition(objectName: string,
      filterName: string): Observable<IListDefinitionColumn[]> {
    const simpleListRequest: IListDefinitionRequest = {
      objectName, filterName
    };
    const key = this.keyListDefinition(objectName, filterName);
    const obs = this.httpClient
      .post<IListDefinitionColumn[]>(this.configurationService.apiListDefinition, simpleListRequest);

    return this.asyncCache.wrap(obs, key, { } );
  }

  deleteListDefinition(objectName: string,
      filterName: string): void {
    this.cacheDriver.delete(this.keyListDefinition(objectName, filterName));
  }

  keyListDefinition(objectName: string,
      filterName: string): string {
    return 'FL_' + objectName + '_' + filterName;
  }

  listRequestObservable(objectName: string, filterName: string): Observable<ListRequest> {
    this.ensure(objectName, filterName);

    return this.requestsObservable[this.key(objectName, filterName)];
  }

  listResponseObservable(objectName: string, filterName: string): Observable<IListResponse> {
    this.ensure(objectName, filterName);

    return this.resultsObservable[this.key(objectName, filterName)];
  }

  listBusyObservable(objectName: string, filterName: string): Observable<boolean> {
    this.ensure(objectName, filterName);

    return this.busyObservable[this.key(objectName, filterName)];
  }

  private sendListRequest(key: string) {
    const request = this._requestsSubject[key].value;

    this._busySubject[key].next(true);
    this.httpClient
      .post<IListResponse>(this.configurationService.apiCrudPost, request)
      .subscribe(resp => {
        this._busySubject[key].next(false);
        this._resultsSubject[this.key(request.objectName, request.filterName)].next(resp);
      }, error => {
        this._busySubject[key].next(false);
      });
  }

  private ensure(objectName: string, filterName: string): void {
    const key = this.key(objectName, filterName);

    if (!this._resultsSubject[key]) {
      this._resultsSubject[key] = new BehaviorSubject<IListResponse>(null);
      this.resultsObservable[key] = this._resultsSubject[key].asObservable();

      this._requestsSubject[key] = new BehaviorSubject<ListRequest>(new ListRequest());
      this.requestsObservable[key] = this._requestsSubject[key].asObservable();

      this._busySubject[key] = new BehaviorSubject<boolean>(false);
      this.busyObservable[key] = this._busySubject[key].asObservable();

      this.sendListRequest(key);
    }
  }

  key(objectName: string, filterName: string): string {
    return objectName + '_' + filterName;
  }
}
