import { InjectionToken, NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { EditCarComponent } from './edit-car/edit-car.component';
import { DeleteCarComponent } from './delete-car/delete-car.component';
import { ListCarComponent } from './list-car/list-car.component';
import { ICarManager } from 'src/services/abstractions/i-car-manager.interface';
import { LocalApiCarManager } from 'src/services/local-api-car-manager.service';
import { HttpService } from 'src/services/http-service.service';
import { HttpClientModule } from '@angular/common/http';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { FormsModule } from '@angular/forms';
import {CreateCarComponent} from "./create-car/create-car.component";

export const SERVICE_KEYS = {
  HttpService: new InjectionToken<HttpService>('HttpService'),
  ICarManager: new InjectionToken<ICarManager>('ICarManager'),
};

@NgModule({
  declarations: [
    AppComponent,
    ListCarComponent,
    CreateCarComponent,
    EditCarComponent,
    DeleteCarComponent,
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    HttpClientModule,
    FontAwesomeModule,
    FormsModule,
  ],
  providers: [
    {
      provide: SERVICE_KEYS.ICarManager,
      useClass: LocalApiCarManager,
    },
    {
      provide: SERVICE_KEYS.HttpService,
      useClass: HttpService,
    },
  ],
  bootstrap: [AppComponent],
})
export class AppModule {}
