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
import { IDictionary } from 'protr/protr';

export class ProtrEditorService {
  protected _currentDecoratorSubject: BehaviorSubject<Decorator>;
  protected _initCompletedSubject: BehaviorSubject<boolean>;
  protected _currentBusinessObjectSubject: BehaviorSubject<BusinessObject>;
  protected _currentCrudResponseSubject: BehaviorSubject<ICrudResponse>;
  protected _busySubject: BehaviorSubject<boolean>;

  public currentDecoratorObservable: Observable<Decorator>;
  public initCompletedObservable: Observable<boolean>;
  public currentBusinessObjectObservable: Observable<BusinessObject>;
  public currentCrudResponseObservable: Observable<ICrudResponse>;
  public busyObservable: Observable<boolean>;

  public currentBusinessObject: BusinessObject;

  private initDecoratorObservablesCount: number;
  private initDecoratorObservablesNotified: number;
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
    this._currentCrudResponseSubject = new BehaviorSubject<ICrudResponse>(null);
    this._busySubject = new BehaviorSubject<boolean>(false);

    this.currentDecoratorObservable = this._currentDecoratorSubject.asObservable();
    this.initCompletedObservable = this._initCompletedSubject.asObservable();
    this.currentBusinessObjectObservable = this._currentBusinessObjectSubject.asObservable();
    this.currentCrudResponseObservable = this._currentCrudResponseSubject.asObservable();
    this.busyObservable = this._busySubject.asObservable();
  }

  public init(name: string, creator: () => BusinessObject) {
    this.creator = creator;
    this.configurationService
      .getProperties(name)
      .then(resp => {
        console.log('Init object type ' + name);
        this.initDecoratorObservablesCount = this._currentDecoratorSubject.observers.length;
        this.initDecoratorObservablesNotified = 0;
        this.fieldNames = [];
        this.pendingInitSubject = true;
        this._currentDecoratorSubject.next(new Decorator(name, resp));
      });
  }

  public registerFieldNamesOnInit(fieldNames: string[]) {
    this.initDecoratorObservablesNotified++;

    fieldNames.forEach(fieldName => {
      if (this.fieldNames.indexOf(fieldName) === -1) {
        this.fieldNames.push(fieldName);
        console.log('New field: ' + fieldName);
      }
    });

    if (this.pendingInitSubject && this.initDecoratorObservablesNotified === this.initDecoratorObservablesCount) {
      this.pendingInitSubject = false;
      console.log('All Observables notified for ' + this._currentDecoratorSubject.value.name);

      this.crudRequest = new CrudRequest();
      this.crudRequest.dataNames = this.fieldNames;
      this.crudRequest.objectName = this._currentDecoratorSubject.value.name;
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

  public load(key: string) {
    this.crudRequest.root.key = key;
    this._busySubject.next(true);
    this.protrCrudService
      .load(this.crudRequest)
      .then(resp => {
        this._busySubject.next(false);
        this.crudResponse = resp;
        this._currentCrudResponseSubject.next(this.crudResponse);
        this.setCurrentBusinessObject(this.crudResponse.data);
      })
      .catch(error => {
        this._busySubject.next(false);
      });
  }

  public save() {
    this.crudRequest.root.data = this.currentBusinessObject.ToDictionary();
    this._busySubject.next(true);
    this.protrCrudService
      .save(this.crudRequest)
      .then(resp => {
        this._busySubject.next(false);
        this.crudResponse = resp;
        this._currentCrudResponseSubject.next(this.crudResponse);
        this.setCurrentBusinessObject(this.crudResponse.data);
      })
      .catch(error => {
        this._busySubject.next(false);
      });
  }

  public cancel() {
    this._busySubject.next(true);
    this.protrCrudService
      .load(this.crudRequest)
      .then(resp => {
        this._busySubject.next(false);
        this.crudResponse = resp;
        this._currentCrudResponseSubject.next(this.crudResponse);
        this.setCurrentBusinessObject(this.crudResponse.data);
      })
      .catch(error => {
        this._busySubject.next(false);
      });
  }

  public setModified() {
    if (this.crudResponse && !this.crudResponse.isModified) {
      this.crudResponse.isModified = true;
      this._currentCrudResponseSubject.next(this.crudResponse);
    }
    if (this.currentBusinessObject) {
      this._currentBusinessObjectSubject.next(this.currentBusinessObject);
    }
  }

  public isBusy() {
    return this._busySubject.value;
  }

  protected setCurrentBusinessObject(businessObject: IDictionary<any>) {
    if (businessObject != null) {
      if (this.currentBusinessObject == null) {
        this.currentBusinessObject = this.creator();
      }
      this.currentBusinessObject = Object.assign(this.currentBusinessObject, businessObject);
    } else {
      this.currentBusinessObject = this.creator();
    }
    this._currentBusinessObjectSubject.next(this.currentBusinessObject);
  }
}
