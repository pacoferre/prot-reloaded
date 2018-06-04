import { Injectable } from '@angular/core';
import { Observable, BehaviorSubject } from 'rxjs';
import { User } from '../dtos/user';
import { ConfigurationService } from './configuration.service';
import { ProtrUser, ProtrAuthenticationService, ProtrEditorService } from 'protr';
import { HttpClient } from '@angular/common/http';
import { AuthenticationService } from './authentication.service';

@Injectable()
export class EditorService extends ProtrEditorService {
  constructor(httpClient: HttpClient, configurationService: ConfigurationService, authenticationService: AuthenticationService) {
    super(httpClient, configurationService, authenticationService);
  }
}
