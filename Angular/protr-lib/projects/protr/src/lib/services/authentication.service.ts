import { Injectable } from '@angular/core';
import { Observable, BehaviorSubject } from 'rxjs';
import { ProtrUser } from '../dtos/user';
import { ProtrConfigurationService } from './configuration.service';
import { HttpClient } from '@angular/common/http';

@Injectable()
export class ProtrAuthenticationService {
  protected _currentUserSubject: BehaviorSubject<ProtrUser>;

  constructor(protected httpClient: HttpClient, protected configurationService: ProtrConfigurationService) {
  }

  currentUser(): Promise<ProtrUser> {
    return new Promise<ProtrUser>(resolve => {
      if (this.isAuthenticated) {
        return resolve(this._currentUserSubject.value);
      }

      const currentUserObservable = this.httpClient
        .get<ProtrUser>(this.configurationService.apiCurrentUser);
      currentUserObservable
        .subscribe(resp => {
          if (resp != null) {
            const user = Object.assign(this.createBlankUser(), resp);
            this._currentUserSubject.next(user);
            resolve(user);
          } else {
            this._currentUserSubject.next(null);
            resolve(null);
          }
        }, error => {
          this._currentUserSubject.next(null);
          resolve(null);
        });
    });
  }

  login(email: string, password: string): Promise<boolean> {
    return new Promise<boolean>(resolve => {
      this.httpClient
        .post<ProtrUser>(this.configurationService.apiLogin, { email: email, password: password })
        .subscribe(resp => {
          if (resp != null) {
            this._currentUserSubject.next(Object.assign(this.createBlankUser(), resp));
            resolve(true);
          } else {
            this._currentUserSubject.next(null);
            resolve(false);
          }
        }, error => {
          this._currentUserSubject.next(null);
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

  createBlankUser(): ProtrUser {
    return new ProtrUser();
  }
}
