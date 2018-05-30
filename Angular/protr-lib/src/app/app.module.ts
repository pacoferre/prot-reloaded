import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';

import { AppComponent } from './app.component';
import { ProtrModule } from 'protr';

@NgModule({
  declarations: [
    AppComponent
  ],
  imports: [
    BrowserModule,
    ProtrModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
