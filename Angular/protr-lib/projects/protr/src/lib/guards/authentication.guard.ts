import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { Observable } from 'rxjs';
import { ProtrAuthenticationService } from '../services/authentication.service';

export class ProtrAuthGuard implements CanActivate {
  constructor(protected authService: ProtrAuthenticationService) {
  }

  canActivate(next: ActivatedRouteSnapshot, state: RouterStateSnapshot):
      Observable<boolean> | Promise<boolean> | boolean {
    if (!this.authService.isAuthenticated) {
      return new Promise<boolean>(resolve => {
        this.authService
          .currentUser()
          .then(current => {
            resolve(current != null);
            if (current == null) {
              this.doFail();
            }
          });
      });
    }

    return true;
  }

  doFail() {
  }
}
