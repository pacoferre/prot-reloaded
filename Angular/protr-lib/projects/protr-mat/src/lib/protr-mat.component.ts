import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'lib-protr-mat',
  template: `
  <mat-form-field>
    <input matInput placeholder="Favorite food" value="Sushi">
  </mat-form-field>
  `,
  styles: []
})
export class ProtrMatComponent implements OnInit {

  constructor() { }

  ngOnInit() {
  }

}
