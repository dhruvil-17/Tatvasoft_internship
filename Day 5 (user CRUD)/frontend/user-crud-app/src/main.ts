import { bootstrapApplication } from '@angular/platform-browser';
import { App } from './app/app';
import { importProvidersFrom } from '@angular/core';
import { provideHttpClient } from '@angular/common/http';
import { provideRouter } from '@angular/router';
import { UserCrudComponent } from './app/user-crud/user-crud';

bootstrapApplication(App, {
  providers: [
    provideHttpClient(),
    provideRouter([{ path: '', component: UserCrudComponent }])
  ]
});
