import { Injectable } from '@angular/core';
import { ProtrConfigurationService } from './configuration.service';
import { HttpClient } from '@angular/common/http';
import { AsyncCache, CacheDriver } from 'angular-async-cache';
import { Observable, BehaviorSubject } from 'rxjs';
import { IListDefinitionRequest } from '../dtos/listDefinitionRequest';
import { IListDefinitionColumn } from '../dtos/listDefinitionColumn';
import { ListResponse } from '../dtos/listResponse';
import { ListRequest } from '../dtos/listRequest';
import { IDictionary } from 'protr/protr';

export class ProtrFilteringService {
  protected cacheDriver: CacheDriver;
  private _resultsSubject: IDictionary<BehaviorSubject<ListResponse>> = {};
  private resultsObservable: IDictionary<Observable<ListResponse>> = {};
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
    const key = this.filterKey(objectName, filterName);

    return this.requestsObservable[key];
  }

  listResponseObservable(objectName: string, filterName: string): Observable<ListResponse> {
    const key = this.filterKey(objectName, filterName);

    return this.resultsObservable[key];
  }

  listBusyObservable(objectName: string, filterName: string): Observable<boolean> {
    const key = this.filterKey(objectName, filterName);

    return this.busyObservable[key];
  }

  setPageNumberSorting(objectName: string, filterName: string,
      pageNumber: number, rowsPerPage: number, sortDir: string, sortIndex: number) {
    const key = this.filterKey(objectName, filterName);
    const request = this._requestsSubject[key].value;

    if (pageNumber !== undefined) {
      request.pageNumber = pageNumber;
    }
    if (rowsPerPage !== undefined) {
      request.rowsPerPage = rowsPerPage;
    }
    if (sortDir !== undefined) {
      request.sortDir = sortDir;
    }
    if (sortIndex !== undefined) {
      request.sortIndex = sortIndex;
    }

    this.sendListRequest(key, request);
  }

  private ensure(objectName: string, filterName: string, key: string): void {
    if (!this._resultsSubject[key]) {
      const request = new ListRequest();

      request.objectName = objectName;
      request.filterName = filterName;

      this._resultsSubject[key] = new BehaviorSubject<ListResponse>(null);
      this.resultsObservable[key] = this._resultsSubject[key].asObservable();

      this._requestsSubject[key] = new BehaviorSubject<ListRequest>(request);
      this.requestsObservable[key] = this._requestsSubject[key].asObservable();

      this._busySubject[key] = new BehaviorSubject<boolean>(false);
      this.busyObservable[key] = this._busySubject[key].asObservable();

      this.sendListRequest(key, request);
    }
  }

  private sendListRequest(key: string, request: ListRequest) {
    this._busySubject[key].next(true);
    this.httpClient
      .post<ListResponse>(this.configurationService.apiList, request)
      .subscribe(resp => {
        resp = Object.assign(new ListResponse(), resp);

        this._busySubject[key].next(false);
        this._resultsSubject[key].next(resp);

        request.pageNumber = resp.pageNumber;
        request.rowsPerPage = resp.rowsPerPage;
        request.sortDir = resp.sortDir;
        request.sortIndex = resp.sortIndex;
        request.filters = resp.filters;
        request.fastsearch = resp.fastsearch;
        request.topRecords = resp.topRecords;

        this._requestsSubject[key].next(request);
      }, error => {
        this._busySubject[key].next(false);
      });
  }

  filterKey(objectName: string, filterName: string): string {
    const theKey = objectName + '_' + filterName;
    this.ensure(objectName, filterName, theKey);
    return theKey;
  }
}
