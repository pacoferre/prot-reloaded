import { Injectable } from '@angular/core';
import { Http } from '@angular/http';
import { Observable, BehaviorSubject } from 'rxjs';
import { User } from '../dtos/user';
import { ConfigurationService } from './configuration.service';
import { ProtrUser, ProtrAuthService } from 'protr';

@Injectable()
export class AuthenticationService extends ProtrAuthService {
  constructor(http: Http, configurationService: ConfigurationService) {
    super(http, configurationService);
  }

  createBlankUser(): ProtrUser {
    return new User();
  }
}
