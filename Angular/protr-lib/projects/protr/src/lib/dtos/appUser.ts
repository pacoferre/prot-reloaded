import { BusinessObject } from './businessObject';

export class ProtrAppUser extends BusinessObject {
  idAppUser: string;
  name: string;
  surname: string;
  su: boolean;
  email: string;
}
