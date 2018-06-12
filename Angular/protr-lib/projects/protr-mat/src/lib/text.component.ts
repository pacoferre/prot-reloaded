import { Component, Inject, Input } from '@angular/core';
import { FieldControl, ProtrEditorService, Decorator, BusinessObject } from 'protr';
import { FormControl, Validators } from '@angular/forms';

@Component({
  selector: 'pmat-text',
  template: `
  <mat-form-field>
    <input matInput #input [placeholder]="label" [maxlength]="maxLength"
        [formControl]="control">
    <mat-hint align="end">{{input.value?.length || 0}}/{{maxLength}}</mat-hint>
    <mat-error *ngIf="isInvalid()">
      {{invalidText}}
    </mat-error>
  </mat-form-field>
  `,
  styles: []
})
export class ProtrMatTextComponent extends FieldControl {
  maxLength: number;

  constructor(@Inject('EditorService') protected protrEditorService: ProtrEditorService) {
    super(protrEditorService);
  }

  prepare(decorator: Decorator) {
    const props = decorator.fieldProperties[this.fieldName];

    this.maxLength = props.maxLength;

    super.prepare(decorator);
  }
}
