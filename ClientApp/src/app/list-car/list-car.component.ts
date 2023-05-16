import { Component, Inject, OnInit } from '@angular/core';
import { Car } from 'src/models/car.model';
import { SERVICE_KEYS } from '../app.module';
import { ICarManager } from 'src/services/abstractions/i-car-manager.interface';
import { faTrashCan, faPenSquare } from '@fortawesome/free-solid-svg-icons';

@Component({
  selector: 'app-list-car',
  templateUrl: './list-car.component.html',
  styleUrls: ['./list-car.component.css'],
})
export class ListCarComponent implements OnInit {
  faTrashCan = faTrashCan;
  faPenSquare = faPenSquare;

  public cars?: Car[];
  constructor(
    @Inject(SERVICE_KEYS.ICarManager) private carManager: ICarManager
  ) {}

  async ngOnInit() {
    this.cars = await this.carManager.getCars();
  }
}
