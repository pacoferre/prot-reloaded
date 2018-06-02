import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { Observable } from 'rxjs';
import { ProtrAuthService } from '../services/authentication.service';

@Injectable()
export class ProtrAuthGuard implements CanActivate {
  constructor(protected authService: ProtrAuthService) {
  }

  canActivate(next: ActivatedRouteSnapshot, state: RouterStateSnapshot):
      Observable<boolean> | Promise<boolean> | boolean {
    if (!this.authService.isAuthenticated) {
      // try authenticate when session ready or any other mechanism
      this.doFail();
      return false;
    }

    return true;
  }

  doFail() {
  }
}
