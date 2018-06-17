import { IPermission } from './crudResponse';
import { IDictionary } from '../interfaces/IDictionary';
import { IListDefinitionColumn } from './listDefinitionColumn';

export class ListResponse {
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

  toNamed(cols: IListDefinitionColumn[]): any[] {
    return this.result.map(row => {
      const resp: any = {};

      for (let index = 0; index < row.length; ++index) {
        resp[cols[index].as] = row[index];
      }

      return resp;
    });
  }
}
