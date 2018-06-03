import { Component, OnInit } from '@angular/core';
import { ProtrEditorDirective, IDictionary, IFieldProperty, Decorator } from 'protr';

@Component({
  selector: 'app-hello',
  templateUrl: './hello.component.html',
})
export class HelloComponent {
  decorator: Decorator;

  constructor(
    private parentComponent: ProtrEditorDirective,
  ) {
    parentComponent
      .ready
      .asObservable()
      .subscribe(decorator => {
        const that = this;
        this.decorator = this.parentComponent.decorator;
      });
  }
}
