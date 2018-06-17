import { IDictionary } from '../interfaces/IDictionary';

export class ListRequest {
  objectName: string;
  filterName: string;
  sortIndex: number;
  sortDir: string;
  dofastsearch = false;
  fastsearch: string;
  filters: IDictionary<string> = {};
  pageNumber = 1;
  rowsPerPage = 100;
  topRecords = 100;
  first = true;
}
