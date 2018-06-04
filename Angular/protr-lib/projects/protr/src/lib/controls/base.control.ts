import { OnInit, OnDestroy } from '@angular/core';
import { ProtrEditorService } from '../services/editor.service';
import { Decorator } from '../dtos/decorator';
import { BusinessObject } from '../dtos/businessObject';
import { Subscription } from 'rxjs';

export class BaseControl implements OnInit, OnDestroy {
  private subDecorator: Subscription;
  private subBusinessObject: Subscription;

  constructor(protected protrEditorService: ProtrEditorService) {
  }

  ngOnInit(): void {
    this.subDecorator = this.protrEditorService.currentDecoratorObserver()
      .subscribe(d => {
        if (d != null) {
          this.prepare(d);
        }
      });

    this.subBusinessObject = this.protrEditorService.currentBusinessObjectObserver()
      .subscribe(b => {
        this.load(b);
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

  prepare(decorator: Decorator) {
  }

  load(businessObject: BusinessObject) {
  }

  modified() {
    this.protrEditorService.setModified();
  }
}
