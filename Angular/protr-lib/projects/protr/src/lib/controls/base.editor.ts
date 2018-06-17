import { ProtrEditorService } from '../services/editor.service';
import { BaseControl } from './base.control';
import { Decorator } from '../dtos/decorator';
import { BusinessObject } from '../dtos/businessObject';
import { OnInit, OnDestroy } from '@angular/core';
import { Subscription } from 'rxjs';

export class BaseEditor extends BaseControl implements OnInit, OnDestroy {
  constructor(protected objectName: string,
      protected creator: () => BusinessObject,
      public protrEditorService: ProtrEditorService) {
    super(protrEditorService);
  }

  private subInitComplete: Subscription;

  ngOnInit() {
    super.ngOnInit();

    this.subInitComplete = this.protrEditorService.initCompletedObservable
      .subscribe(ready => {
        if (ready) {
          this.ready();
        }
      });

    this.protrEditorService
      .init(this.objectName, this.creator);
  }

  ngOnDestroy() {
    super.ngOnDestroy();

    if (this.subInitComplete) {
      this.subInitComplete.unsubscribe();
    }
  }

  ready() {
    console.log('Editor ready');
  }

  load(key: string) {
    this.protrEditorService.load(key);
  }
}
