import { Injectable } from '@angular/core';
import { ProtrAuthGuard } from 'protr';
import { Router } from '@angular/router';
import { AuthenticationService } from '../services/authentication.service';

@Injectable()
export class AuthGuard extends ProtrAuthGuard {
  constructor(private router: Router, authService: AuthenticationService) {
    super(authService);
  }

  doFail() {
    this.router.navigate(['login']);
  }
}
