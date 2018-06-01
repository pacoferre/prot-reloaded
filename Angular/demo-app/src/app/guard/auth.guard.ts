import { Injectable } from '@angular/core';
import { ProtrAuthGuard } from 'protr';
import { Router } from '@angular/router';

@Injectable()
export class AuthGuard extends ProtrAuthGuard {
  constructor(private router: Router) {
    super();
  }

  doFail() {
    this.router.navigate(['login']);
  }
}
