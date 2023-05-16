export class CarDTO {
  constructor(
    public model: string = '',
    public manufacturer: string = '',
    public price: number = 0,
    public year: number = 0
  ) {}
}
