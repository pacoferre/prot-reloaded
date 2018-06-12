// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: authentication.proto
// </auto-generated>
#pragma warning disable 1591
#region Designer generated code

using grpc = global::Grpc.Core;

namespace PROTR.GRPC.Authentication {
  public static partial class Login
  {
    static readonly string __ServiceName = "PROTR.gRPC.Authentication.Login";

    static readonly grpc::Marshaller<global::PROTR.GRPC.Authentication.LoginRequest> __Marshaller_LoginRequest = grpc::Marshallers.Create((arg) => global::Google.Protobuf.MessageExtensions.ToByteArray(arg), global::PROTR.GRPC.Authentication.LoginRequest.Parser.ParseFrom);
    static readonly grpc::Marshaller<global::PROTR.GRPC.Authentication.AppUser> __Marshaller_AppUser = grpc::Marshallers.Create((arg) => global::Google.Protobuf.MessageExtensions.ToByteArray(arg), global::PROTR.GRPC.Authentication.AppUser.Parser.ParseFrom);

    static readonly grpc::Method<global::PROTR.GRPC.Authentication.LoginRequest, global::PROTR.GRPC.Authentication.AppUser> __Method_AskLogin = new grpc::Method<global::PROTR.GRPC.Authentication.LoginRequest, global::PROTR.GRPC.Authentication.AppUser>(
        grpc::MethodType.Unary,
        __ServiceName,
        "AskLogin",
        __Marshaller_LoginRequest,
        __Marshaller_AppUser);

    /// <summary>Service descriptor</summary>
    public static global::Google.Protobuf.Reflection.ServiceDescriptor Descriptor
    {
      get { return global::PROTR.GRPC.Authentication.AuthenticationReflection.Descriptor.Services[0]; }
    }

    /// <summary>Base class for server-side implementations of Login</summary>
    public abstract partial class LoginBase
    {
      public virtual global::System.Threading.Tasks.Task<global::PROTR.GRPC.Authentication.AppUser> AskLogin(global::PROTR.GRPC.Authentication.LoginRequest request, grpc::ServerCallContext context)
      {
        throw new grpc::RpcException(new grpc::Status(grpc::StatusCode.Unimplemented, ""));
      }

    }

    /// <summary>Client for Login</summary>
    public partial class LoginClient : grpc::ClientBase<LoginClient>
    {
      /// <summary>Creates a new client for Login</summary>
      /// <param name="channel">The channel to use to make remote calls.</param>
      public LoginClient(grpc::Channel channel) : base(channel)
      {
      }
      /// <summary>Creates a new client for Login that uses a custom <c>CallInvoker</c>.</summary>
      /// <param name="callInvoker">The callInvoker to use to make remote calls.</param>
      public LoginClient(grpc::CallInvoker callInvoker) : base(callInvoker)
      {
      }
      /// <summary>Protected parameterless constructor to allow creation of test doubles.</summary>
      protected LoginClient() : base()
      {
      }
      /// <summary>Protected constructor to allow creation of configured clients.</summary>
      /// <param name="configuration">The client configuration.</param>
      protected LoginClient(ClientBaseConfiguration configuration) : base(configuration)
      {
      }

      public virtual global::PROTR.GRPC.Authentication.AppUser AskLogin(global::PROTR.GRPC.Authentication.LoginRequest request, grpc::Metadata headers = null, global::System.DateTime? deadline = null, global::System.Threading.CancellationToken cancellationToken = default(global::System.Threading.CancellationToken))
      {
        return AskLogin(request, new grpc::CallOptions(headers, deadline, cancellationToken));
      }
      public virtual global::PROTR.GRPC.Authentication.AppUser AskLogin(global::PROTR.GRPC.Authentication.LoginRequest request, grpc::CallOptions options)
      {
        return CallInvoker.BlockingUnaryCall(__Method_AskLogin, null, options, request);
      }
      public virtual grpc::AsyncUnaryCall<global::PROTR.GRPC.Authentication.AppUser> AskLoginAsync(global::PROTR.GRPC.Authentication.LoginRequest request, grpc::Metadata headers = null, global::System.DateTime? deadline = null, global::System.Threading.CancellationToken cancellationToken = default(global::System.Threading.CancellationToken))
      {
        return AskLoginAsync(request, new grpc::CallOptions(headers, deadline, cancellationToken));
      }
      public virtual grpc::AsyncUnaryCall<global::PROTR.GRPC.Authentication.AppUser> AskLoginAsync(global::PROTR.GRPC.Authentication.LoginRequest request, grpc::CallOptions options)
      {
        return CallInvoker.AsyncUnaryCall(__Method_AskLogin, null, options, request);
      }
      /// <summary>Creates a new instance of client from given <c>ClientBaseConfiguration</c>.</summary>
      protected override LoginClient NewInstance(ClientBaseConfiguration configuration)
      {
        return new LoginClient(configuration);
      }
    }

    /// <summary>Creates service definition that can be registered with a server</summary>
    /// <param name="serviceImpl">An object implementing the server-side handling logic.</param>
    public static grpc::ServerServiceDefinition BindService(LoginBase serviceImpl)
    {
      return grpc::ServerServiceDefinition.CreateBuilder()
          .AddMethod(__Method_AskLogin, serviceImpl.AskLogin).Build();
    }

  }
}
#endregion
