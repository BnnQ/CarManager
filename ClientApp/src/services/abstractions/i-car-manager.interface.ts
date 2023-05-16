import { Car } from 'src/models/car.model';
import { CarDTO } from 'src/models/dto/car-dto.model';

export interface ICarManager {
  getCar(id: string): Promise<Car | undefined>;
  getCars(): Promise<Car[] | undefined>;

  createCar(car: CarDTO): Promise<void>;
  editCar(id: string, editedCar: CarDTO): Promise<void>;
  deleteCar(id: string): Promise<void>;
}
