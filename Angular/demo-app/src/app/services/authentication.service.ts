import { Injectable } from '@angular/core';
import { Observable, BehaviorSubject } from 'rxjs';
import { User } from '../dtos/user';
import { ConfigurationService } from './configuration.service';
import { ProtrUser, ProtrAuthenticationService } from 'protr';
import { HttpClient } from '@angular/common/http';

@Injectable()
export class AuthenticationService extends ProtrAuthenticationService {
  constructor(httpClient: HttpClient, configurationService: ConfigurationService) {
    super(httpClient, configurationService);

    this._currentUserSubject = <BehaviorSubject<ProtrUser>>new BehaviorSubject<User>(null);
  }

  get currentUserSubject(): BehaviorSubject<User> {
    return <BehaviorSubject<User>>this._currentUserSubject;
  }

  createBlankUser(): ProtrUser {
    return new User();
  }
}
