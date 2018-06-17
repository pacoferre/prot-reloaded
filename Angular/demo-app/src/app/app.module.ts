import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { Routes, RouterModule } from '@angular/router';
import { HttpModule } from '@angular/http';
import { HttpClientModule } from '@angular/common/http';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { ProtrMatModule } from 'protr-mat';

import { AppComponent } from './app.component';
import { LoginComponent } from './components/login/login.component';
import { StartComponent } from './components/start/start.component';

import { AuthGuard } from './guards/authentication.guard';

import { AuthenticationService } from './services/authentication.service';
import { ConfigurationService } from './services/configuration.service';
import { EditorService } from './services/editor.service';
import { CrudService } from './services/crud.service';
import { SimpleListService } from './services/simpleList.service';
import {
  AsyncCacheModule,
  AsyncCacheOptions,
  MemoryDriver
} from 'angular-async-cache';
import { FilteringService } from './services/filtering.service';
import { UserListComponent } from './components/users/user.list.component';
import { AuthorListComponent } from './components/authors/author.list.component';

export function asyncCacheOptionsFactory(): AsyncCacheOptions {
  return new AsyncCacheOptions({
    // Default cache driver to use. Default in memory.
    // You can also roll your own by implementing the CacheDriver interface
    driver: new MemoryDriver(),

    // this is the special sauce - first emit the data from localstorage,
    // then re-fetch the live data from the API and emit a second time.
    // The async pipe will then re-render and update the UI. Default: false
    fromCacheAndReplay: false
  });
}

const routes: Routes = [
  { path: 'start', component: StartComponent, canActivate: [ AuthGuard ] },
  { path: 'users', component: UserListComponent, canActivate: [ AuthGuard ] },
  { path: 'authors', component: AuthorListComponent, canActivate: [ AuthGuard ] },
  { path: 'login', component: LoginComponent },
  { path: '', component : StartComponent, canActivate: [ AuthGuard ],  }
];

@NgModule({
  declarations: [
    AppComponent,
    LoginComponent,
    StartComponent,
    UserListComponent,
    AuthorListComponent
  ],
  imports: [
    BrowserModule,
    BrowserAnimationsModule,
    FormsModule,
    ReactiveFormsModule,
    ProtrMatModule,
    RouterModule.forRoot(routes),
    HttpModule,
    HttpClientModule,
    AsyncCacheModule.forRoot({
      provide: AsyncCacheOptions,
      useFactory: asyncCacheOptionsFactory
    })
  ],
  providers: [
    AuthGuard,
    ConfigurationService,
    AuthenticationService,
    { provide: 'EditorService', useClass: EditorService },
    { provide: 'CrudService', useClass: CrudService },
    { provide: 'SimpleListService', useClass: SimpleListService },
    { provide: 'FilteringService', useClass: FilteringService }
  ],
  bootstrap: [ AppComponent ]
})
export class AppModule { }
