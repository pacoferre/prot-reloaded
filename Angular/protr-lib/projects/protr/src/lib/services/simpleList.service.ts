import { Injectable } from '@angular/core';
import { ProtrConfigurationService } from './configuration.service';
import { HttpClient } from '@angular/common/http';
import { ISimpleListRequest } from '../dtos/simpleListRequest';
import { ISimpleListResponseItem } from '../dtos/simpleListResponse';
import { AsyncCache, MemoryDriver } from 'angular-async-cache';
import { Observable } from 'rxjs';

@Injectable()
export class ProtrSimpleListService {
  constructor(protected httpClient: HttpClient,
    protected configurationService: ProtrConfigurationService,
    protected asyncCache: AsyncCache
  ) {
  }

  get(objectName: string,
      listName: string,
      parameter: string): Observable<ISimpleListResponseItem[]> {
    const simpleListRequest: ISimpleListRequest = {
      objectName, listName, parameter
    };
    const key = this.key(objectName, listName, parameter);
    const obs = this.httpClient
      .post<ISimpleListResponseItem[]>(this.configurationService.apiCrudPost, simpleListRequest);

    return this.asyncCache.wrap(obs, key, { } );
  }

  delete(objectName: string,
      listName: string,
      parameter: string): void {
    throw new Error('This method must be overrided');
  }

  key(objectName: string,
      listName: string,
      parameter: string): string {
    return 'SL_' + objectName + '_' + listName + '_' + parameter;
  }
}
