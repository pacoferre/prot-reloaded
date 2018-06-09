import { Component, OnInit, Inject } from '@angular/core';
import { ProtrEditorService, BaseEditor } from 'protr';
import { User } from '../dtos/user';

@Component({
  selector: 'app-start',
  templateUrl: './start.component.html',
  styleUrls: ['./start.component.scss']
})
export class StartComponent extends BaseEditor {
  constructor(@Inject('EditorService') protrEditorService: ProtrEditorService) {
    super('AppUser', () => new User(), protrEditorService);
  }
}
