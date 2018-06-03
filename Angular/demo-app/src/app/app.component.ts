import { Component, ViewEncapsulation } from '@angular/core';
import { AuthenticationService } from './services/authentication.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class AppComponent {
  wellcome = '';

  constructor(private authenticationService: AuthenticationService, private router: Router) {
    authenticationService
      .currentUserSubject
      .subscribe(user => {
        if (user != null) {
          this.wellcome = 'Hello ' + user.name + ' ' + user.surname;
        }
      });
  }

  logout() {
    this.authenticationService
      .logout()
      .then(r => {
        this.router.navigate(['login']);
      });
  }
}
