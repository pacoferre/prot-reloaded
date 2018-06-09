import { IDictionary } from '../interfaces/IDictionary';

export interface IPermission {
  modify: boolean;
  delete: boolean;
  add: boolean;
}

export interface ICrudResponse {
  permission: IPermission;
  collections: IDictionary<ICrudResponse>;
  formToken: string;
  sequence: number;
  action: string;
  ok: boolean;
  keyObject: string;
  keyObjectUp: string;
  keyObjectDown: string;
  keysObjectInternal?: any;
  goBack: boolean;
  wasModified: boolean;
  wasNew: boolean;
  wasDeleting: boolean;
  isModified: boolean;
  isNew: boolean;
  isDeleting: boolean;
  data: IDictionary<any>;
  props?: any;
  tools?: any;
  refreshLists?: any;
  title: string;
  errorMessage: string;
  errorCollection: string;
  errorCollectionKey: string;
  errorProperty: string;
  normalMessage: string;
  resultData?: any;
  resultOpenURL?: any;
  filterPosition: number;
  refreshFilterTable: boolean;
  refreshAll: boolean;
  refreshAllLists: boolean;
}

