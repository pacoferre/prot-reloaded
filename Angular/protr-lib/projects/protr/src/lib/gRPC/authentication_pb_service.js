// package: PROTR.gRPC.Authentication
// file: authentication.proto

var authentication_pb = require("./authentication_pb");
var grpc = require("grpc-web-client").grpc;

var Login = (function () {
  function Login() {}
  Login.serviceName = "PROTR.gRPC.Authentication.Login";
  return Login;
}());

Login.AskLogin = {
  methodName: "AskLogin",
  service: Login,
  requestStream: false,
  responseStream: false,
  requestType: authentication_pb.LoginRequest,
  responseType: authentication_pb.AppUser
};

exports.Login = Login;

function LoginClient(serviceHost, options) {
  this.serviceHost = serviceHost;
  this.options = options || {};
}

LoginClient.prototype.askLogin = function askLogin(requestMessage, metadata, callback) {
  if (arguments.length === 2) {
    callback = arguments[1];
  }
  grpc.unary(Login.AskLogin, {
    request: requestMessage,
    host: this.serviceHost,
    metadata: metadata,
    transport: this.options.transport,
    onEnd: function (response) {
      if (callback) {
        if (response.status !== grpc.Code.OK) {
          callback(Object.assign(new Error(response.statusMessage), { code: response.status, metadata: response.trailers }), null);
        } else {
          callback(null, response.message);
        }
      }
    }
  });
};

exports.LoginClient = LoginClient;

