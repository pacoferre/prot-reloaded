import { Injectable, Inject } from '@angular/core';
import { Observable, BehaviorSubject } from 'rxjs';
import { User } from '../dtos/user';
import { ConfigurationService } from './configuration.service';
import { ProtrUser, ProtrAuthenticationService, ProtrEditorService } from 'protr';
import { HttpClient } from '@angular/common/http';
import { AuthenticationService } from './authentication.service';
import { CrudService } from './crud.service';

@Injectable()
export class EditorService extends ProtrEditorService {
  constructor(httpClient: HttpClient,
    configurationService: ConfigurationService,
    authenticationService: AuthenticationService,
    @Inject('CrudService') crudService: CrudService) {
    super(httpClient, configurationService, authenticationService, crudService);
  }
}
