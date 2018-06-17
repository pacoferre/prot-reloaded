import { CollectionViewer, DataSource } from '@angular/cdk/collections';
import { BehaviorSubject, Observable, Subscription } from 'rxjs';
import { ProtrFilteringService } from 'protr';

export class ListDataSource implements DataSource<any[]> {
  private resultsSubject = new BehaviorSubject<any[][]>([]);
  private sub: Subscription;

  constructor(private protrFilteringService: ProtrFilteringService, private objectName: string, private filterName: string) {
  }

  connect(collectionViewer: CollectionViewer): Observable<any[][]> {
    this.sub = this.protrFilteringService
      .listResponseObservable(this.objectName, this.filterName)
      .subscribe(items => this.resultsSubject.next(items.result));

    return this.resultsSubject.asObservable();
  }

  disconnect(collectionViewer: CollectionViewer): void {
    this.resultsSubject.complete();

    if (this.sub) {
      this.sub.unsubscribe();
    }
  }
}
