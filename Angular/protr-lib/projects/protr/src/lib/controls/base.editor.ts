import { ProtrEditorService } from '../services/editor.service';
import { BaseControl } from './base.control';
import { Decorator } from '../dtos/decorator';

export class BaseEditor extends BaseControl {
  constructor(protected objectName: string, protected protrEditorService: ProtrEditorService) {
    super(protrEditorService);

    this.protrEditorService
      .init(this.objectName);
  }

  prepare(decorator: Decorator) {
    super.prepare(decorator);

    window.setTimeout(this.ready.bind(this), 1);
  }

  ready() {
  }
}
