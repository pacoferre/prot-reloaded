import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { Routes, RouterModule } from '@angular/router';
import { HttpModule } from '@angular/http';

import { ProtrMatModule } from 'protr-mat';

import { AppComponent } from './app.component';
import { LoginComponent } from './login/login.component';
import { StartComponent } from './start/start.component';

import { AuthGuard } from './guards/authentication.guard';
import { AuthenticationService } from './services/authentication.service';
import { ConfigurationService } from './services/configuration.service';


const routes: Routes = [
  { path: 'start', component: StartComponent, canActivate: [ AuthGuard ] },
  { path: 'login', component: LoginComponent },
  { path: '', component : LoginComponent }
];

@NgModule({
  declarations: [
    AppComponent,
    LoginComponent,
    StartComponent
  ],
  imports: [
    BrowserModule,
    BrowserAnimationsModule,
    ProtrMatModule,
    RouterModule.forRoot(routes),
    HttpModule
  ],
  providers: [
    AuthGuard,
    ConfigurationService,
    AuthenticationService
  ],
  bootstrap: [ AppComponent ]
})
export class AppModule { }
