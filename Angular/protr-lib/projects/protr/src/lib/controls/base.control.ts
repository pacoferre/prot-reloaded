import { OnInit, OnDestroy, Input } from '@angular/core';
import { ProtrEditorService } from '../services/editor.service';
import { Decorator } from '../dtos/decorator';
import { BusinessObject } from '../dtos/businessObject';
import { Subscription } from 'rxjs';

export class BaseControl implements OnInit, OnDestroy {
  private subDecorator: Subscription;
  private subBusinessObject: Subscription;
  private fieldNames: string[];

  constructor(protected protrEditorService: ProtrEditorService) {
  }

  ngOnInit(): void {
    this.subDecorator = this.protrEditorService.currentDecoratorObserver
      .subscribe(d => {
        if (d != null) {
          this.fieldNames = [];
          this.prepare(d);
          this.protrEditorService.registerFieldNamesOnInit(this.fieldNames);
        }
      });

    this.subBusinessObject = this.protrEditorService.currentBusinessObjectObserver
      .subscribe(b => {
        this.read(b);
      });
  }

  ngOnDestroy(): void {
    if (this.subDecorator) {
      this.subDecorator.unsubscribe();
    }
    if (this.subBusinessObject) {
      this.subBusinessObject.unsubscribe();
    }
  }

  registerFieldNamesOnInit(fieldNames: string[]) {
    fieldNames.forEach(fieldName => {
      if (this.fieldNames.indexOf(fieldName) === -1) {
        this.fieldNames.push(fieldName);
      }
    });
  }

  prepare(decorator: Decorator) {
  }

  read(businessObject: BusinessObject) {
  }

  modified() {
    this.protrEditorService.setModified();
  }
}
