import { Component, OnInit, Inject } from '@angular/core';
import { ProtrEditorService, BaseEditor } from 'protr';
import { User } from '../dtos/user';
import { FormGroup } from '@angular/forms';

@Component({
  selector: 'app-start',
  templateUrl: './start.component.html',
  styleUrls: ['./start.component.scss']
})
export class StartComponent extends BaseEditor {

  constructor(@Inject('EditorService') protrEditorService: ProtrEditorService) {
    super('AppUser', protrEditorService);
  }

  ready() {
    const model = new User();

    model.name = 'Hello';
    model.surname = 'World';

    this.protrEditorService.setCurrentBusinessObject(model);
  }
}
