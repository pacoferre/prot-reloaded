import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AuthenticationService } from '../services/authentication.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html'
})
export class LoginComponent {
  constructor(private router: Router, private authenticationService: AuthenticationService) {
  }

  login(email: string, password: string): void {
    this.authenticationService
      .login(email, password)
      .then(resp => {
        if (resp) {
          this.router.navigate(['']);
        }
      });
  }
}
