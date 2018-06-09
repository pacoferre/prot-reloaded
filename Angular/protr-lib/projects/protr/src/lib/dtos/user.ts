import { BusinessObject } from './businessObject';

export class ProtrUser extends BusinessObject {
  id: string;
  name: string;
  surname: string;
  su: boolean;
  email: string;
}
