import { OnInit, OnDestroy, Input } from '@angular/core';
import { ProtrEditorService } from '../services/editor.service';
import { Decorator } from '../dtos/decorator';
import { BusinessObject } from '../dtos/businessObject';
import { Subscription } from 'rxjs';
import { BaseControl } from './base.control';

export class FieldControl extends BaseControl {
  @Input()
  fieldName: string;

  constructor(protected protrEditorService: ProtrEditorService) {
    super(protrEditorService);
  }

  prepare(decorator: Decorator) {
  }

  load(businessObject: BusinessObject) {
  }
}
