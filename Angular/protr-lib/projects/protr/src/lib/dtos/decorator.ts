import { IDictionary } from '../interfaces/IDictionary';

export interface IFieldProperty {
  type: number;
  isIdentity: boolean;
  isReadOnly: boolean;
  isOnlyOnNew: boolean;
  fieldName: string;
  label: string;
  labelIsFieldName: boolean;
  clientFormat: string;
  pattern: string;
  maxLength: number;
  required: boolean;
  requiredErrorMessage: string;
  noLabelRequired: boolean;
  noChecking: boolean;
  min: string;
  max: string;
  step: string;
  listObjectName: string;
  listName: string;
  isObjectView: boolean;
  listAjax: boolean;
  rows: number;
  defaultSearch: string;
  searchMultipleSelect: boolean;
  alwaysFloatLabel: boolean;
}

export class Decorator {
  public name: string;
  public fieldProperties: IDictionary<IFieldProperty>;
  public fieldPropertiesArray: IFieldProperty[];

  constructor(name: string, fieldProperties: IDictionary<IFieldProperty>) {
    this.name = name;
    this.fieldProperties = fieldProperties;
    this.fieldPropertiesArray = Object
      .keys(fieldProperties)
      .map(function(key) {
        return fieldProperties[key];
    });

  }
}
