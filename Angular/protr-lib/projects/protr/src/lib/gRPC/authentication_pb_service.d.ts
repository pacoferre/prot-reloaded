// package: PROTR.gRPC.Authentication
// file: authentication.proto

import * as authentication_pb from "./authentication_pb";
import {grpc} from "grpc-web-client";

type LoginAskLogin = {
  readonly methodName: string;
  readonly service: typeof Login;
  readonly requestStream: false;
  readonly responseStream: false;
  readonly requestType: typeof authentication_pb.LoginRequest;
  readonly responseType: typeof authentication_pb.AppUser;
};

export class Login {
  static readonly serviceName: string;
  static readonly AskLogin: LoginAskLogin;
}

export type ServiceError = { message: string, code: number; metadata: grpc.Metadata }
export type Status = { details: string, code: number; metadata: grpc.Metadata }
export type ServiceClientOptions = { transport: grpc.TransportConstructor }

interface ResponseStream<T> {
  cancel(): void;
  on(type: 'data', handler: (message: T) => void): ResponseStream<T>;
  on(type: 'end', handler: () => void): ResponseStream<T>;
  on(type: 'status', handler: (status: Status) => void): ResponseStream<T>;
}

export class LoginClient {
  readonly serviceHost: string;

  constructor(serviceHost: string, options?: ServiceClientOptions);
  askLogin(
    requestMessage: authentication_pb.LoginRequest,
    metadata: grpc.Metadata,
    callback: (error: ServiceError, responseMessage: authentication_pb.AppUser|null) => void
  ): void;
  askLogin(
    requestMessage: authentication_pb.LoginRequest,
    callback: (error: ServiceError, responseMessage: authentication_pb.AppUser|null) => void
  ): void;
}

