import { IPermission } from './crudResponse';
import { IDictionary } from '../interfaces/IDictionary';

export interface IListResponse {
  plural: string;
  permission: IPermission;
  result: any[][];
  fastsearch: string;
  sortIndex: number;
  sortDir: string;
  filters: IDictionary<string>;
  pageNumber: number;
  rowsPerPage: number;
  topRecords: number;
  rowCount: number;
}
