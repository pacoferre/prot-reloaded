import { Component, Inject, OnInit, OnDestroy, ViewChild, AfterViewInit } from '@angular/core';
import { BaseList, ProtrFilteringService, ProtrEditorService, IListDefinitionColumn } from 'protr';
import { ListDataSource } from './listDataSource';
import { MatPaginator, MatSort } from '@angular/material';
import { tap } from 'rxjs/operators';
import { merge } from 'rxjs';

@Component({
  selector: 'pmat-list',
  template: `
<ng-container *ngIf="dataSource">
  <mat-table
      [dataSource]="dataSource" matSort matSortActive="C1" matSortDirection="asc" matSortDisableClear>
    <ng-container [matColumnDef]="col.as" *ngFor="let col of columns; index as i">
      <mat-header-cell *matHeaderCellDef mat-sort-header>{{col.label}}</mat-header-cell>
      <mat-cell *matCellDef="let item">{{item[col.as]}}</mat-cell>
    </ng-container>
    <mat-header-row *matHeaderRowDef="displayColumns"></mat-header-row>
    <mat-row *matRowDef="let row; columns: displayColumns"
      (click)="onRowClicked(row)">
    </mat-row>
  </mat-table>

  <div *ngIf="busyObservable | async">
    <mat-spinner></mat-spinner>
  </div>

  <mat-paginator [length]="resultsLength" [pageSize]="100"
                    [pageSizeOptions]="[100, 500, 1000]"></mat-paginator>
</ng-container>
`
})
export class ProtrMatListComponent extends BaseList implements OnInit, OnDestroy, AfterViewInit {
  dataSource: ListDataSource;
  columns: IListDefinitionColumn[] = [];
  displayColumns: string[] = [];
  resultsLength = 0;
  subscribed = false;

  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;

  constructor(@Inject('EditorService') protrEditorService: ProtrEditorService,
      @Inject('FilteringService') protrFilteringService: ProtrFilteringService) {
    super(protrEditorService, protrFilteringService);
  }

  ngOnInit() {
    super.ngOnInit();

    this.columnsObservable.subscribe(cols => {
      this.columns = cols.filter(col => !col.isID);
      this.displayColumns = this.columns.map(col => col.as);
      this.dataSource = new ListDataSource(this.protrFilteringService, this.objectName, this.filterName, cols);

      this.dataSource.pageCountObservable.subscribe(count => {
        this.resultsLength = count;
        if (!this.subscribed && this.sort && this.paginator) {
          this.subscribed = true;
          this.sort.sortChange.subscribe(() => this.paginator.pageIndex = 0);
          merge(this.sort.sortChange, this.paginator.page)
            .pipe(tap(() => {
                this.protrFilteringService
                  .setPageNumberSorting(this.objectName, this.filterName,
                    this.paginator.pageIndex,
                    this.paginator.pageSize,
                    this.sort.direction,
                    parseInt(this.sort.active, 10));
              })
            )
            .subscribe();
        }
      });
    });
  }

  ngAfterViewInit() {
  }

  ngOnDestroy() {
    super.ngOnDestroy();
  }

  onRowClicked(row) {
    console.log('Row clicked: ', row);
  }
}
