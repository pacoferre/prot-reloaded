import { Decorator } from './decorator';
import { IDictionary } from '../interfaces/IDictionary';

export class BusinessObject {
  public decorator: Decorator;

  public BusinessObject() {
  }

  public ToDictionary(): IDictionary<any> {
    return Object.assign({}, this);
  }
}
