import { Component, OnInit, Inject } from '@angular/core';
import { FieldControl, ProtrEditorService, Decorator, BusinessObject } from 'protr';

@Component({
  selector: 'pmat-text',
  template: `
  <mat-form-field>
    <input matInput #input [placeholder]="label" [(ngModel)]="value" [maxlength]="maxLength">
    <mat-hint align="end">{{input.value?.length || 0}}/{{maxLength}}</mat-hint>
  </mat-form-field>
  `,
  styles: []
})
export class ProtrMatTextComponent extends FieldControl {
  value: string;
  maxLength: number;
  label: string;

  constructor(@Inject('EditorService') protected protrEditorService: ProtrEditorService) {
    super(protrEditorService);
  }

  prepare(decorator: Decorator) {
    const props = decorator.fieldProperties[this.fieldName];

    this.value = '';
    this.maxLength = props.maxLength;
    this.label = props.label;
  }

  load(businessObject: BusinessObject) {
    if (businessObject == null) {
      this.value = '';
    } else {
      this.value = businessObject[this.fieldName];
    }
  }
}
