import { Input, Output, EventEmitter } from '@angular/core';
import { ProtrEditorService } from '../services/editor.service';
import { BaseControl } from './base.control';
import { FormControl, Validators } from '@angular/forms';
import { BusinessObject } from '../dtos/businessObject';
import { Decorator } from '../dtos/decorator';

export class FieldControl extends BaseControl {
  @Input() fieldName: string;
  control: FormControl;
  label: string;

  constructor(protected protrEditorService: ProtrEditorService) {
    super(protrEditorService);

    this.control = new FormControl('');

    this.control.valueChanges.subscribe(value => this.changed());
  }

  changed() {
    if (this.protrEditorService && this.protrEditorService.currentBusinessObject) {
      this.protrEditorService.currentBusinessObject[this.fieldName] = this.control.value;
      this.protrEditorService.setModified();
    }
  }

  prepare(decorator: Decorator) {
    const props = decorator.fieldProperties[this.fieldName];

    this.label = props.label;
    if (props.required) {
      this.control.setValidators([Validators.required]);
    }

    this.registerFieldNamesOnInit([ this.fieldName ]);
  }

  load(businessObject: BusinessObject): void {
    this.control.reset();
    if (businessObject != null) {
      this.control.setValue(businessObject[this.fieldName]);
    }

    super.load(businessObject);
  }

  isInvalid(): boolean {
    return !this.control.valid && this.control.touched;
  }
}
