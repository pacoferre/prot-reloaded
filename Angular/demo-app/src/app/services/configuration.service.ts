import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { ProtrConfigurationService } from 'protr';
import { HttpClient } from '@angular/common/http';

@Injectable()
export class ConfigurationService extends ProtrConfigurationService {
  constructor(httpClient: HttpClient) {
    super(httpClient);

    this.loadFromEnvironment(environment);
  }
}
