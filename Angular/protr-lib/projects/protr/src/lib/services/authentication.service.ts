import { Injectable } from '@angular/core';
import { Observable, BehaviorSubject } from 'rxjs';
import { ProtrAppUser } from '../dtos/appUser';
import { ProtrConfigurationService } from './configuration.service';
import { HttpClient } from '@angular/common/http';

export class ProtrAuthenticationService {
  protected _currentUserSubject: BehaviorSubject<ProtrAppUser>;

  constructor(protected httpClient: HttpClient, protected configurationService: ProtrConfigurationService) {
  }

  currentUserObserver() {
    return this._currentUserSubject.asObservable();
  }

  currentUser(): Promise<ProtrAppUser> {
    return new Promise<ProtrAppUser>(resolve => {
      if (this.isAuthenticated) {
        return resolve(this._currentUserSubject.value);
      }

      this.httpClient
        .get<ProtrAppUser>(this.configurationService.apiCurrentUser)
        .subscribe(resp => {
          if (resp != null) {
            const user = Object.assign(this.createBlankUser(), resp);
            this.setCurrentUser(user);
            resolve(user);
          } else {
            this.setCurrentUser(null);
            resolve(null);
          }
        }, error => {
          this.setCurrentUser(null);
          resolve(null);
        });
    });
  }

  setCurrentUser(user: ProtrAppUser) {
    this._currentUserSubject.next(user);
  }

  login(email: string, password: string): Promise<boolean> {
    return new Promise<boolean>(resolve => {
      this.httpClient
        .post<ProtrAppUser>(this.configurationService.apiLogin, { email: email, password: password })
        .subscribe(resp => {
          if (resp != null) {
            const user = Object.assign(this.createBlankUser(), resp);
            this.setCurrentUser(user);
            resolve(true);
          } else {
            this.setCurrentUser(null);
            resolve(false);
          }
        }, error => {
          this.setCurrentUser(null);
          resolve(false);
        });
    });
  }

  logout(): Promise<boolean> {
    return new Promise<boolean>(resolve => {
      this.httpClient
        .get(this.configurationService.apiLogout)
        .subscribe(resp => {
          this._currentUserSubject.next(null);
          resolve(true);
        }, error => {
          this._currentUserSubject.next(null);
          resolve(true);
        });
    });
  }

  get isAuthenticated(): boolean {
    return this._currentUserSubject.value != null;
  }

  createBlankUser(): ProtrAppUser {
    return new ProtrAppUser();
  }
}
