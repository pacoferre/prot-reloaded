import { Injectable } from '@angular/core';
import { Http } from '@angular/http';
import { Observable, BehaviorSubject } from 'rxjs';
import { ProtrUser } from '../dtos/user';
import { ProtrConfigurationService } from './configuration.service';

@Injectable()
export class ProtrAuthService {
  constructor(protected http: Http, protected configurationService: ProtrConfigurationService) { }

  currentUser = new BehaviorSubject<ProtrUser>(null);

  login(username: string, password: string): void {
    this.http
      .post(this.configurationService.apiLogin, { username: username, password: password })
      .subscribe(resp => {
        this.currentUser.next(Object.assign(this.createBlankUser(), resp.json));
      }, error => {
        this.currentUser.next(null);
      });
  }

  logout() {
    this.http
      .get(this.configurationService.apiLogout)
      .subscribe(resp => {
        this.currentUser.next(null);
      }, error => {
        this.currentUser.next(null);
      });
  }

  get isAuthenticated(): boolean {
    return this.currentUser.value != null;
  }

  createBlankUser(): ProtrUser {
    return new ProtrUser();
  }
}
