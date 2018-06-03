import { Injectable } from '@angular/core';
import { Observable, BehaviorSubject } from 'rxjs';
import { ProtrUser } from '../dtos/user';
import { ProtrConfigurationService } from './configuration.service';
import { HttpClient } from '@angular/common/http';

@Injectable()
export class ProtrAuthenticationService {
  currentUserSubject = new BehaviorSubject<ProtrUser>(null);
  _currentUserObservable: Observable<ProtrUser>;

  constructor(protected httpClient: HttpClient, protected configurationService: ProtrConfigurationService) {
    this._currentUserObservable = this.currentUserSubject.asObservable();
  }

  get currentUserObservable() {
    return this._currentUserObservable;
  }

  currentUser(): Promise<ProtrUser> {
    return new Promise<ProtrUser>(resolve => {
      if (this.isAuthenticated) {
        return resolve(this.currentUserSubject.value);
      }

      const currentUserObservable = this.httpClient
        .get<ProtrUser>(this.configurationService.apiCurrentUser);
      currentUserObservable
        .subscribe(resp => {
          if (resp != null) {
            const user = Object.assign(this.createBlankUser(), resp);
            this.currentUserSubject.next(user);
            resolve(user);
          } else {
            this.currentUserSubject.next(null);
            resolve(null);
          }
        }, error => {
          this.currentUserSubject.next(null);
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
            this.currentUserSubject.next(Object.assign(this.createBlankUser(), resp));
            resolve(true);
          } else {
            this.currentUserSubject.next(null);
            resolve(false);
          }
        }, error => {
          this.currentUserSubject.next(null);
          resolve(false);
        });
    });
  }

  logout(): Promise<boolean> {
    return new Promise<boolean>(resolve => {
      this.httpClient
        .get(this.configurationService.apiLogout)
        .subscribe(resp => {
          this.currentUserSubject.next(null);
          resolve(true);
        }, error => {
          this.currentUserSubject.next(null);
          resolve(true);
        });
    });
  }

  get isAuthenticated(): boolean {
    return this.currentUserSubject.value != null;
  }

  createBlankUser(): ProtrUser {
    return new ProtrUser();
  }
}
