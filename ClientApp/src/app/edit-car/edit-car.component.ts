import { Component, Inject } from '@angular/core';
import { SERVICE_KEYS } from '../app.module';
import { ICarManager } from 'src/services/abstractions/i-car-manager.interface';
import { CarDTO } from 'src/models/dto/car-dto.model';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-edit-car',
  templateUrl: './edit-car.component.html',
  styleUrls: ['./edit-car.component.css'],
})
export class EditCarComponent {
  public car: CarDTO = new CarDTO();
  private id: string = '';

  constructor(
    @Inject(SERVICE_KEYS.ICarManager) private carManager: ICarManager,
    private router: Router,
    activatedRoute: ActivatedRoute
  ) {
    activatedRoute.params.subscribe(async (params) => {
      this.id = params['id'];

      const car$ = await carManager.getCar(this.id);
      if (!car$) {
        this.navigateToList();
        return;
      } else {
        const car = car$;
        this.car.model = car.model;
        this.car.manufacturer = car.manufacturer;
        this.car.price = car.price;
        this.car.year = car.year;
      }
    });
  }

  public async editCar(car: CarDTO) {
    await this.carManager.editCar(this.id, car);

    this.navigateToList();
  }

  private navigateToList(): void {
    this.router.navigate(['/']);
  }
}
