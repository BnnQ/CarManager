import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { CreateCarComponent } from './create-car/create-car.component';
import { EditCarComponent } from './edit-car/edit-car.component';
import { DeleteCarComponent } from './delete-car/delete-car.component';
import { ListCarComponent } from './list-car/list-car.component';

const routes: Routes = [
  { path: '', component: ListCarComponent },
  { path: 'cars/create', component: CreateCarComponent },
  { path: 'cars/edit/:id', component: EditCarComponent },
  { path: 'cars/delete/:id', component: DeleteCarComponent },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
})
export class AppRoutingModule {}
