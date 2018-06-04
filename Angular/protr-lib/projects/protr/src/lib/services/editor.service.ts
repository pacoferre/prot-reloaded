import { Injectable } from '@angular/core';
import { Observable, BehaviorSubject } from 'rxjs';
import { ProtrUser } from '../dtos/user';
import { ProtrConfigurationService } from './configuration.service';
import { HttpClient } from '@angular/common/http';
import { ProtrAuthenticationService } from './authentication.service';
import { BusinessObject } from '../dtos/businessObject';
import { Decorator } from '../dtos/decorator';

@Injectable()
export class ProtrEditorService {
  protected _currentDecoratorSubject: BehaviorSubject<Decorator>;
  protected _currentBusinessObjectSubject: BehaviorSubject<BusinessObject>;
  protected _modifiedSubject: BehaviorSubject<boolean>;
  protected currentBusinessObject: BusinessObject;

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

  private setCurrentBusinessObject(businessObject: BusinessObject) {
    this._currentBusinessObjectSubject.next(null);
    this.currentBusinessObject = null;
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
}
