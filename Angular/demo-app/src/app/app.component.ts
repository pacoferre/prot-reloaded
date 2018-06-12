import { Component, ViewEncapsulation, Inject, OnInit } from '@angular/core';
import { AuthenticationService } from './services/authentication.service';
import { Router } from '@angular/router';
import { MatSnackBar } from '@angular/material';
import { ProtrEditorService } from 'protr';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class AppComponent implements OnInit {
  wellcome = '';

  constructor(private authenticationService: AuthenticationService, private router: Router,
      private snackBar: MatSnackBar,
      @Inject('EditorService') private protrEditorService: ProtrEditorService
  ) {
  }

  ngOnInit() {

    this.authenticationService
      .currentUserSubject
      .subscribe(user => {
        if (user != null) {
          this.wellcome = 'Hello ' + user.name + ' ' + user.surname;
        }
      });

    this.protrEditorService
      .currentCrudResponseObserver
      .subscribe(resp => {
        if (resp != null) {
          if (resp.errorMessage != null && resp.errorMessage !== '') {
            this.snackBar.open(resp.normalMessage, 'Ok', {
              duration: 2000,
            });
          } else if (resp.normalMessage != null && resp.normalMessage !== '') {
            this.snackBar.open(resp.normalMessage, 'Ok', {
              duration: 3000,
            });
          }
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
