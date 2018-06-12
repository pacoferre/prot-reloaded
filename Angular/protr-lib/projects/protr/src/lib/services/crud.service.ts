import { Injectable } from '@angular/core';
import { ProtrConfigurationService } from './configuration.service';
import { HttpClient } from '@angular/common/http';
import { CrudRequest } from '../dtos/crudRequest';
import { ICrudResponse } from '../dtos/crudResponse';

@Injectable()
export class ProtrCrudService {
  constructor(protected httpClient: HttpClient, protected configurationService: ProtrConfigurationService) {
  }

  init(crudRequest: CrudRequest): Promise<ICrudResponse> {
    crudRequest.action = 'init';

    return new Promise<ICrudResponse>(resolve => {
      this.httpClient
        .post<ICrudResponse>(this.configurationService.apiCrudPost, crudRequest)
        .subscribe(resp => {
          resolve(resp);
        }, error => {
          resolve(null);
        });
    });
  }

  load(crudRequest: CrudRequest): Promise<ICrudResponse> {
    crudRequest.action = 'load';

    return new Promise<ICrudResponse>(resolve => {
      this.httpClient
        .post<ICrudResponse>(this.configurationService.apiCrudPost, crudRequest)
        .subscribe(resp => {
          resolve(resp);
        }, error => {
          resolve(null);
        });
    });
  }

  save(crudRequest: CrudRequest): Promise<ICrudResponse> {
    crudRequest.action = 'ok';

    return new Promise<ICrudResponse>(resolve => {
      this.httpClient
        .post<ICrudResponse>(this.configurationService.apiCrudPost, crudRequest)
        .subscribe(resp => {
          resolve(resp);
        }, error => {
          resolve(null);
        });
    });
  }
}
