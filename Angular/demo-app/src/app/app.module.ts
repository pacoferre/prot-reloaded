import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';

import { AppComponent } from './app.component';
import { ProtrModule } from 'protr';
import { ProtrMatModule } from 'protr-mat';

import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

@NgModule({
  declarations: [
    AppComponent
  ],
  imports: [
    BrowserModule,
    BrowserAnimationsModule,
    ProtrModule,
    ProtrMatModule
  ],
  providers: [],
  bootstrap: [ AppComponent ]
})
export class AppModule { }
