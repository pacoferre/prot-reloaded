import { Component, Inject, Input } from '@angular/core';
import { FieldControl, ProtrEditorService, Decorator, BusinessObject, ProtrSimpleListService, ISimpleListResponseItem } from 'protr';
import { FormControl, Validators } from '@angular/forms';
import { Observable } from 'rxjs';

@Component({
  selector: 'pmat-dropdown',
  template: `
<mat-form-field>
  <mat-select [placeholder]="label" [formControl]="control">
    <mat-option *ngFor="let item of items | async" value="{{ item.key }}">{{ item.text }}</mat-option>
  </mat-select>
  <mat-error *ngIf="isInvalid()">
    {{invalidText}}
  </mat-error>
</mat-form-field>
  `,
  styles: []
})
export class ProtrMatDropdownComponent extends FieldControl {
  constructor(@Inject('EditorService') protected protrEditorService: ProtrEditorService,
      @Inject('SimpleListService') protected protrSimpleListService: ProtrSimpleListService) {
    super(protrEditorService);
  }

  items: Observable<ISimpleListResponseItem[]>;

  prepare(decorator: Decorator) {
    const props = decorator.fieldProperties[this.fieldName];

    this.items = this.protrSimpleListService.get(props.listObjectName, props.listName, '0');

    super.prepare(decorator);
  }
}
