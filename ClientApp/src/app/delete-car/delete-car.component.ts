import { Component, Inject } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Car } from 'src/models/car.model';
import { SERVICE_KEYS } from '../app.module';
import { ICarManager } from 'src/services/abstractions/i-car-manager.interface';

@Component({
  selector: 'app-delete-car',
  templateUrl: './delete-car.component.html',
  styleUrls: ['./delete-car.component.css'],
})
export class DeleteCarComponent {
  public car: Car = new Car();

  constructor(
    @Inject(SERVICE_KEYS.ICarManager) private carManager: ICarManager,
    private router: Router,
    activatedRoute: ActivatedRoute
  ) {
    activatedRoute.params.subscribe(async (params) => {
      const car$ = await carManager.getCar(params['id']);
      if (car$ === undefined) this.navigateToList();
      else this.car = car$;
    });
  }

  public async deleteCar() {
    await this.carManager.deleteCar(this.car.id);

    this.navigateToList();
  }

  private navigateToList() {
    this.router.navigate(['/']);
  }
}
