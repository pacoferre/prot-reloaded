import { Component, Inject, OnInit, OnDestroy } from '@angular/core';
import { BaseList, ProtrFilteringService, ProtrEditorService } from 'protr';
import { ListDataSource } from './listDataSource';

@Component({
  selector: 'pmat-list',
  templateUrl: './list.component.html',
  styles: []
})
export class ProtrMatListComponent extends BaseList implements OnInit, OnDestroy {

  dataSource: ListDataSource;
  displayedColumns = [];

  constructor(@Inject('EditorService') protrEditorService: ProtrEditorService,
      @Inject('FilteringService') protrFilteringService: ProtrFilteringService) {
    super(protrEditorService, protrFilteringService);
  }

  ngOnInit() {
    super.ngOnInit();

    this.dataSource = new ListDataSource(this.protrFilteringService, this.objectName, this.filterName);
  }

  ngOnDestroy() {
    super.ngOnDestroy();
  }

  onRowClicked(row) {
    console.log('Row clicked: ', row);
  }
}
