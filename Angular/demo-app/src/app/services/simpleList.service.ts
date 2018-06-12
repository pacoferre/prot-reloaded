import { Injectable, Inject } from '@angular/core';
import { ConfigurationService } from './configuration.service';
import { ProtrSimpleListService } from 'protr';
import { HttpClient } from '@angular/common/http';
import { AsyncCache, MemoryDriver } from 'angular-async-cache';

@Injectable()
export class SimpleListService extends ProtrSimpleListService {
  constructor(httpClient: HttpClient,
      configurationService: ConfigurationService,
      asyncCache: AsyncCache,
      private memoryDriver: MemoryDriver) {
    super(httpClient, configurationService, asyncCache);
  }

  delete(objectName: string,
      listName: string,
      parameter: string): void {
    this.memoryDriver.delete(this.key(objectName, listName, parameter));
  }
}
