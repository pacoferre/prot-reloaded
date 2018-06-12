// package: PROTR.gRPC.Authentication
// file: authentication.proto

import * as jspb from "google-protobuf";

export class AppUser extends jspb.Message {
  getIdappuser(): number;
  setIdappuser(value: number): void;

  getName(): string;
  setName(value: string): void;

  getSurname(): string;
  setSurname(value: string): void;

  getSu(): boolean;
  setSu(value: boolean): void;

  getEmail(): string;
  setEmail(value: string): void;

  getPassword(): string;
  setPassword(value: string): void;

  getDeactivated(): boolean;
  setDeactivated(value: boolean): void;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): AppUser.AsObject;
  static toObject(includeInstance: boolean, msg: AppUser): AppUser.AsObject;
  static extensions: {[key: number]: jspb.ExtensionFieldInfo<jspb.Message>};
  static extensionsBinary: {[key: number]: jspb.ExtensionFieldBinaryInfo<jspb.Message>};
  static serializeBinaryToWriter(message: AppUser, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): AppUser;
  static deserializeBinaryFromReader(message: AppUser, reader: jspb.BinaryReader): AppUser;
}

export namespace AppUser {
  export type AsObject = {
    idappuser: number,
    name: string,
    surname: string,
    su: boolean,
    email: string,
    password: string,
    deactivated: boolean,
  }
}

export class LoginRequest extends jspb.Message {
  getEmail(): string;
  setEmail(value: string): void;

  getPassword(): string;
  setPassword(value: string): void;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): LoginRequest.AsObject;
  static toObject(includeInstance: boolean, msg: LoginRequest): LoginRequest.AsObject;
  static extensions: {[key: number]: jspb.ExtensionFieldInfo<jspb.Message>};
  static extensionsBinary: {[key: number]: jspb.ExtensionFieldBinaryInfo<jspb.Message>};
  static serializeBinaryToWriter(message: LoginRequest, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): LoginRequest;
  static deserializeBinaryFromReader(message: LoginRequest, reader: jspb.BinaryReader): LoginRequest;
}

export namespace LoginRequest {
  export type AsObject = {
    email: string,
    password: string,
  }
}

