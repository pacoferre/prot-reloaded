import { CollectionViewer, DataSource } from '@angular/cdk/collections';
import { BehaviorSubject, Observable, Subscription } from 'rxjs';
import { ProtrFilteringService, IListDefinitionColumn } from 'protr';

export class ListDataSource implements DataSource<any[]> {
  private resultsSubject = new BehaviorSubject<any[]>([]);
  private pageCountSubject = new BehaviorSubject<number>(0);
  private sub: Subscription;

  public pageCountObservable: Observable<number>;

  constructor(private protrFilteringService: ProtrFilteringService,
      private objectName: string, private filterName: string, private cols: IListDefinitionColumn[]) {
    this.pageCountObservable = this.pageCountSubject.asObservable();
  }

  connect(collectionViewer: CollectionViewer): Observable<any[]> {
    this.sub = this.protrFilteringService
      .listResponseObservable(this.objectName, this.filterName)
      .subscribe(items => {
        if (items) {
          this.resultsSubject.next(items.toNamed(this.cols));
          this.pageCountSubject.next(items.rowCount);
        }
      });

    return this.resultsSubject.asObservable();
  }

  disconnect(collectionViewer: CollectionViewer): void {
    this.resultsSubject.complete();

    if (this.sub) {
      this.sub.unsubscribe();
    }
  }
}
