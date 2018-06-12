import { Injectable } from '@angular/core';
import { Observable, BehaviorSubject } from 'rxjs';
import { AppUser } from '../dtos/appUser';
import { ConfigurationService } from './configuration.service';
import { ProtrAppUser, ProtrAuthenticationService } from 'protr';
import { HttpClient } from '@angular/common/http';

@Injectable()
export class AuthenticationService extends ProtrAuthenticationService {
  constructor(httpClient: HttpClient, configurationService: ConfigurationService) {
    super(httpClient, configurationService);

    this._currentUserSubject = <BehaviorSubject<ProtrAppUser>>new BehaviorSubject<AppUser>(null);
  }

  get currentUserSubject(): BehaviorSubject<AppUser> {
    return <BehaviorSubject<AppUser>>this._currentUserSubject;
  }

  createBlankUser(): ProtrAppUser {
    return new AppUser();
  }
}
