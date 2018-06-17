import { Input, OnInit, OnDestroy } from '@angular/core';
import { ProtrEditorService } from '../services/editor.service';
import { BaseControl } from './base.control';
import { IListDefinitionColumn } from '../dtos/listDefinitionColumn';
import { ProtrFilteringService } from '../services/filtering.service';
import { Observable } from 'rxjs';
import { ListResponse } from '../dtos/listResponse';

export class BaseList extends BaseControl implements OnInit, OnDestroy {
  @Input() objectName = '';
  @Input() filterName = '';

  private obsListDefinition: Observable<IListDefinitionColumn[]>;

  constructor(protected protrEditorService: ProtrEditorService, protected protrFilteringService: ProtrFilteringService) {
    super(protrEditorService);
  }

  ngOnInit() {
    super.ngOnInit();
  }

  ngOnDestroy() {
    super.ngOnDestroy();
  }

  get columnsObservable(): Observable<IListDefinitionColumn[]> {
    if (this.objectName === '') {
      return null;
    }
    if (!this.obsListDefinition) {
      this.obsListDefinition = this.protrFilteringService
        .getListDefinition(this.objectName, this.filterName);
    }

    return this.obsListDefinition;
  }

  get responseObservable(): Observable<ListResponse> {
    return this.protrFilteringService.listResponseObservable(this.objectName, this.filterName);
  }

  get busyObservable(): Observable<boolean> {
    return this.protrFilteringService.listBusyObservable(this.objectName, this.filterName);
  }
}
