import { IDictionary } from '../interfaces/IDictionary';

export class CrudBusinessObject {
  key: string;
  data: IDictionary<any>;
  children: Child[];
}

export class Child {
  path: string;
  dataNames: string[];
  elements: CrudBusinessObject[];
}

export class CrudRequest {
  oname: string;
  formToken: string;
  sequence: number;
  action: string;
  dataNames: string[];
  root: CrudBusinessObject;

  constructor() {
    this.root = new CrudBusinessObject();
    this.sequence = 0;
  }
}
