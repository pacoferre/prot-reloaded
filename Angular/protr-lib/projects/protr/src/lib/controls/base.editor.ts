import { ProtrEditorService } from '../services/editor.service';
import { BaseControl } from './base.control';
import { Decorator } from '../dtos/decorator';
import { BusinessObject } from '../dtos/businessObject';

export class BaseEditor extends BaseControl {
  constructor(protected objectName: string,
      protected creator: () => BusinessObject,
      protected protrEditorService: ProtrEditorService) {
    super(protrEditorService);

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
}
