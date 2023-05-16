import { Component, Inject } from '@angular/core';
import { Router } from '@angular/router';
import { CarDTO } from 'src/models/dto/car-dto.model';
import { SERVICE_KEYS } from '../app.module';
import { ICarManager } from 'src/services/abstractions/i-car-manager.interface';

@Component({
  selector: 'app-create-car',
  templateUrl: './create-car.component.html',
  styleUrls: ['./create-car.component.css'],
})
export class CreateCarComponent {
  public car: CarDTO = new CarDTO();

  constructor(
    @Inject(SERVICE_KEYS.ICarManager) private carManager: ICarManager,
    private router: Router
  ) {}

  public async createCar(car: CarDTO) {
    await this.carManager.createCar(car);

    this.navigateToList();
  }

  private navigateToList(): void {
    this.router.navigate(['/']);
  }
}
