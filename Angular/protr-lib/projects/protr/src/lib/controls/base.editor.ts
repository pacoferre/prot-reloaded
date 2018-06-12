import { ProtrEditorService } from '../services/editor.service';
import { BaseControl } from './base.control';
import { Decorator } from '../dtos/decorator';
import { BusinessObject } from '../dtos/businessObject';
import { OnInit } from '@angular/core';

export class BaseEditor extends BaseControl implements OnInit {
  constructor(protected objectName: string,
      protected creator: () => BusinessObject,
      public protrEditorService: ProtrEditorService) {
    super(protrEditorService);
  }

  ngOnInit() {
    super.ngOnInit();

    this.protrEditorService.initCompletedObserver
      .subscribe(ready => {
        if (ready) {
          this.ready();
        }
      });

    this.protrEditorService
      .init(this.objectName, this.creator);
  }

  ready() {
    console.log('Editor ready');
  }

  load(key: string) {
    this.protrEditorService.load(key);
  }
}
