import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { ProtrConfigurationService } from './configuration.service';
import { HttpClient } from '@angular/common/http';
import { ProtrAuthenticationService } from './authentication.service';
import { BusinessObject } from '../dtos/businessObject';
import { Decorator } from '../dtos/decorator';
import { FormGroup, FormControl, Validators } from '@angular/forms';

@Injectable()
export class ProtrEditorService {
  protected _currentDecoratorSubject: BehaviorSubject<Decorator>;
  protected _currentBusinessObjectSubject: BehaviorSubject<BusinessObject>;
  protected _modifiedSubject: BehaviorSubject<boolean>;
  public currentBusinessObject: BusinessObject;

  constructor(protected httpClient: HttpClient,
      protected configurationService: ProtrConfigurationService,
      protected authenticationService: ProtrAuthenticationService) {

    this._currentDecoratorSubject = new BehaviorSubject<Decorator>(null);
    this._currentBusinessObjectSubject = new BehaviorSubject<BusinessObject>(null);
    this._modifiedSubject = new BehaviorSubject<boolean>(false);
  }

  init(name: string) {
    this.configurationService
      .getProperties(name)
      .then(resp => {
        this._currentDecoratorSubject.next(new Decorator(name, resp));
        this.setCurrentBusinessObject(null);
      });
  }

  public setCurrentBusinessObject(businessObject: BusinessObject) {
    this._currentBusinessObjectSubject.next(businessObject);
    this.currentBusinessObject = businessObject;
    this._modifiedSubject.next(false);
  }

  currentDecoratorObserver() {
    return this._currentDecoratorSubject.asObservable();
  }

  currentBusinessObjectObserver() {
    return this._currentBusinessObjectSubject.asObservable();
  }

  load(id: string) {
  }

  setModified() {
    if (!this._modifiedSubject.value) {
      this._modifiedSubject.next(true);
    }
  }

  toFormGroup() {
    const group: any = {};

    this._currentDecoratorSubject.getValue()
      .fieldPropertiesArray
      .forEach(question => {
        group[question.fieldName] = question.required ? new FormControl('', Validators.required)
                                                : new FormControl('');
      });

    return new FormGroup(group);
  }
}
