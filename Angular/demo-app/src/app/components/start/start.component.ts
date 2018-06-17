import { Component, OnInit, Inject } from '@angular/core';
import { ProtrEditorService, BaseEditor } from 'protr';
import { AppUser } from '../../dtos/appUser';
import { AuthenticationService } from '../../services/authentication.service';

@Component({
  selector: 'app-start',
  templateUrl: './start.component.html',
  styleUrls: ['./start.component.scss']
})
export class StartComponent extends BaseEditor {

  fullName = '';

  constructor(@Inject('EditorService') protrEditorService: ProtrEditorService, private authenticationService: AuthenticationService) {
    super('AppUser', () => new AppUser(), protrEditorService);
  }

  ready() {
    super.ready();

    this.protrEditorService.currentBusinessObjectObservable
      .subscribe(bo => {
        if (bo != null) {
          const user = <AppUser>bo;

          this.fullName = user.name + ' ' + user.surname;
        }
      });

    this.authenticationService.currentUser().then(user => this.load(user.idAppUser));
  }
}
