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
  invalidText = '';

  constructor(protected protrEditorService: ProtrEditorService) {
    super(protrEditorService);

    this.control = new FormControl('');

    this.control.valueChanges.subscribe(value => this.changed());
  }

  changed() {
    if (this.protrEditorService && this.protrEditorService.currentBusinessObject) {
      if (this.protrEditorService.currentBusinessObject[this.fieldName] !== this.control.value) {
        this.protrEditorService.currentBusinessObject[this.fieldName] = this.control.value;
        this.protrEditorService.setModified();
      }
    }
  }

  prepare(decorator: Decorator) {
    const props = decorator.fieldProperties[this.fieldName];

    this.label = props.label;
    if (props.required) {
      this.control.setValidators([Validators.required]);

      if (props.requiredErrorMessage != null && props.requiredErrorMessage !== '') {
        this.invalidText = props.requiredErrorMessage;
      } else {
        this.invalidText = this.label + ' is required';
      }
    }

    this.registerFieldNamesOnInit([ this.fieldName ]);
  }

  read(businessObject: BusinessObject): void {
    if (businessObject != null) {
      this.control.setValue(businessObject[this.fieldName], { emitEvent: false });
    }

    super.read(businessObject);
  }

  isInvalid(): boolean {
    return !this.control.valid && this.control.touched;
  }
}
