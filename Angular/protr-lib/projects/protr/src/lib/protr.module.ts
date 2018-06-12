import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import {
  AsyncCacheModule,
  AsyncCacheOptions
} from 'angular-async-cache';

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

@NgModule({
  imports: [
    FormsModule,
    ReactiveFormsModule,
    AsyncCacheModule.forRoot({
      provide: AsyncCacheOptions,
      useFactory: asyncCacheOptionsFactory
    })
  ],
  declarations: [],
  exports: []
})
export class ProtrModule { }
