import { Injectable, Inject } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { ProtrConfigurationService } from './configuration.service';
import { HttpClient } from '@angular/common/http';
import { ProtrAuthenticationService } from './authentication.service';
import { BusinessObject } from '../dtos/businessObject';
import { Decorator } from '../dtos/decorator';
import { ProtrCrudService } from './crud.service';
import { CrudRequest } from '../dtos/crudRequest';
import { ICrudResponse } from '../dtos/crudResponse';

@Injectable()
export class ProtrEditorService {
  protected _currentDecoratorSubject: BehaviorSubject<Decorator>;
  protected _initCompletedSubject: BehaviorSubject<boolean>;
  protected _currentBusinessObjectSubject: BehaviorSubject<BusinessObject>;
  protected _modifiedSubject: BehaviorSubject<boolean>;
  protected _busySubject: BehaviorSubject<boolean>;

  public currentDecoratorObserver: Observable<Decorator>;
  public initCompletedObserver: Observable<boolean>;
  public currentBusinessObjectObserver: Observable<BusinessObject>;
  public modifiedObserver: Observable<boolean>;
  public busyObserver: Observable<boolean>;

  public currentBusinessObject: BusinessObject;

  private initDecoratorObserversCount: number;
  private initDecoratorObserversNotified: number;
  private pendingInitSubject = false;
  private fieldNames: string[];

  protected crudRequest: CrudRequest;
  protected crudResponse: ICrudResponse;
  protected creator: () => BusinessObject;

  constructor(protected httpClient: HttpClient,
      protected configurationService: ProtrConfigurationService,
      protected authenticationService: ProtrAuthenticationService,
      protected protrCrudService: ProtrCrudService) {

    this._currentDecoratorSubject = new BehaviorSubject<Decorator>(null);
    this._initCompletedSubject = new BehaviorSubject<boolean>(false);
    this._currentBusinessObjectSubject = new BehaviorSubject<BusinessObject>(null);
    this._modifiedSubject = new BehaviorSubject<boolean>(false);
    this._busySubject = new BehaviorSubject<boolean>(false);

    this.currentDecoratorObserver = this._currentDecoratorSubject.asObservable();
    this.initCompletedObserver = this._initCompletedSubject.asObservable();
    this.currentBusinessObjectObserver = this._currentBusinessObjectSubject.asObservable();
    this.modifiedObserver = this._modifiedSubject.asObservable();
    this.busyObserver = this._modifiedSubject.asObservable();
  }

  init(name: string, creator: () => BusinessObject) {
    this.creator = creator;
    this.configurationService
      .getProperties(name)
      .then(resp => {
        console.log('Init object type ' + name);
        this.initDecoratorObserversCount = this._currentDecoratorSubject.observers.length;
        this.initDecoratorObserversNotified = 0;
        this.fieldNames = [];
        this.pendingInitSubject = true;
        this._currentDecoratorSubject.next(new Decorator(name, resp));
      });
  }

  registerFieldNamesOnInit(fieldNames: string[]) {
    this.initDecoratorObserversNotified++;

    fieldNames.forEach(fieldName => {
      if (this.fieldNames.indexOf(fieldName) === -1) {
        this.fieldNames.push(fieldName);
        console.log('New field: ' + fieldName);
      }
    });

    if (this.pendingInitSubject && this.initDecoratorObserversNotified === this.initDecoratorObserversCount) {
      this.pendingInitSubject = false;
      console.log('All observers notified for ' + this._currentDecoratorSubject.value.name);
      this.setCurrentBusinessObject(null);

      this.crudRequest = new CrudRequest();
      this.crudRequest.dataNames = this.fieldNames;
      this.crudRequest.oname = this._currentDecoratorSubject.value.name;
      this._busySubject.next(true);
      this.protrCrudService
        .init(this.crudRequest)
        .then(resp => {
          this._busySubject.next(false);
          this.crudResponse = resp;
          this.crudRequest.formToken = resp.formToken;
          this._initCompletedSubject.next(true);
        })
        .catch(error => {
          this._busySubject.next(false);
        });
    }
  }

  public setCurrentBusinessObject(businessObject: BusinessObject) {
    if (businessObject != null) {
      this.currentBusinessObject = Object.assign(this.creator(), businessObject);
    } else {
      this.currentBusinessObject = this.creator();
    }
    this._currentBusinessObjectSubject.next(this.currentBusinessObject);
    this._modifiedSubject.next(false);
  }

  load(id: string) {


  }

  setModified() {
    if (!this._modifiedSubject.value) {
      this._modifiedSubject.next(true);
    }
  }

  isBusy() {
    return this._busySubject.value;
  }
}
