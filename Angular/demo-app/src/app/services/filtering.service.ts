
import { Injectable } from '@angular/core';
import { ConfigurationService } from './configuration.service';
import { ProtrFilteringService } from 'protr';
import { HttpClient } from '@angular/common/http';
import { AsyncCache, MemoryDriver } from 'angular-async-cache';

@Injectable()
export class FilteringService extends ProtrFilteringService {
  constructor(httpClient: HttpClient,
      configurationService: ConfigurationService,
      asyncCache: AsyncCache,
      memoryDriver: MemoryDriver) {
    super(httpClient, configurationService, asyncCache);

    this.cacheDriver = memoryDriver;
  }
}
