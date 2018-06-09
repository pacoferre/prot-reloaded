import { Injectable } from '@angular/core';
import { ConfigurationService } from './configuration.service';
import { ProtrCrudService } from 'protr';
import { HttpClient } from '@angular/common/http';

@Injectable()
export class CrudService extends ProtrCrudService {
  constructor(httpClient: HttpClient, configurationService: ConfigurationService) {
    super(httpClient, configurationService);
  }
}
