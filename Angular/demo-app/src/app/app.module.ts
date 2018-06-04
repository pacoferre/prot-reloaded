import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { Routes, RouterModule } from '@angular/router';
import { HttpModule } from '@angular/http';
import { HttpClientModule } from '@angular/common/http';

import { ProtrModule, ProtrConfigurationService, ProtrAuthenticationService, ProtrEditorService } from 'protr';
import { ProtrMatModule } from 'protr-mat';

import { AppComponent } from './app.component';
import { LoginComponent } from './login/login.component';
import { StartComponent } from './start/start.component';
import { HelloComponent } from './components/shared/hello.component';

import { AuthGuard } from './guards/authentication.guard';

import { AuthenticationService } from './services/authentication.service';
import { ConfigurationService } from './services/configuration.service';
import { EditorService } from './services/editor.service';


const routes: Routes = [
  { path: 'start', component: StartComponent, canActivate: [ AuthGuard ] },
  { path: 'login', component: LoginComponent },
  { path: '', component : StartComponent, canActivate: [ AuthGuard ] }
];

@NgModule({
  declarations: [
    AppComponent,
    LoginComponent,
    StartComponent,
    HelloComponent
  ],
  imports: [
    BrowserModule,
    BrowserAnimationsModule,
    ProtrModule,
    ProtrMatModule,
    RouterModule.forRoot(routes),
    HttpModule,
    HttpClientModule
  ],
  providers: [
    AuthGuard,
    ConfigurationService,
    AuthenticationService,
    { provide: 'EditorService', useClass: EditorService },
  ],
  bootstrap: [ AppComponent ]
})
export class AppModule { }
