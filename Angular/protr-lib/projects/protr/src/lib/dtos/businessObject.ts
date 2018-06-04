import { Decorator } from './decorator';

export class BusinessObject {
  public decorator: Decorator;

  public BusinessObject(decorator: Decorator) {
    this.decorator = decorator;
  }
}
