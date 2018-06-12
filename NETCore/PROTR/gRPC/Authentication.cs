// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: authentication.proto
// </auto-generated>
#pragma warning disable 1591, 0612, 3021
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
namespace PROTR.GRPC.Authentication {

  /// <summary>Holder for reflection information generated from authentication.proto</summary>
  public static partial class AuthenticationReflection {

    #region Descriptor
    /// <summary>File descriptor for authentication.proto</summary>
    public static pbr::FileDescriptor Descriptor {
      get { return descriptor; }
    }
    private static pbr::FileDescriptor descriptor;

    static AuthenticationReflection() {
      byte[] descriptorData = global::System.Convert.FromBase64String(
          string.Concat(
            "ChRhdXRoZW50aWNhdGlvbi5wcm90bxIZUFJPVFIuZ1JQQy5BdXRoZW50aWNh",
            "dGlvbiJ9CgdBcHBVc2VyEhEKCWlkQXBwVXNlchgBIAEoARIMCgRuYW1lGAIg",
            "ASgJEg8KB3N1cm5hbWUYAyABKAkSCgoCc3UYBCABKAgSDQoFZW1haWwYBSAB",
            "KAkSEAoIcGFzc3dvcmQYBiABKAkSEwoLZGVhY3RpdmF0ZWQYByABKAgiLwoM",
            "TG9naW5SZXF1ZXN0Eg0KBWVtYWlsGAEgASgJEhAKCHBhc3N3b3JkGAIgASgJ",
            "MmIKBUxvZ2luElkKCEFza0xvZ2luEicuUFJPVFIuZ1JQQy5BdXRoZW50aWNh",
            "dGlvbi5Mb2dpblJlcXVlc3QaIi5QUk9UUi5nUlBDLkF1dGhlbnRpY2F0aW9u",
            "LkFwcFVzZXIiAGIGcHJvdG8z"));
      descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
          new pbr::FileDescriptor[] { },
          new pbr::GeneratedClrTypeInfo(null, new pbr::GeneratedClrTypeInfo[] {
            new pbr::GeneratedClrTypeInfo(typeof(global::PROTR.GRPC.Authentication.AppUser), global::PROTR.GRPC.Authentication.AppUser.Parser, new[]{ "IdAppUser", "Name", "Surname", "Su", "Email", "Password", "Deactivated" }, null, null, null),
            new pbr::GeneratedClrTypeInfo(typeof(global::PROTR.GRPC.Authentication.LoginRequest), global::PROTR.GRPC.Authentication.LoginRequest.Parser, new[]{ "Email", "Password" }, null, null, null)
          }));
    }
    #endregion

  }
  #region Messages
  public sealed partial class AppUser : pb::IMessage<AppUser> {
    private static readonly pb::MessageParser<AppUser> _parser = new pb::MessageParser<AppUser>(() => new AppUser());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<AppUser> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::PROTR.GRPC.Authentication.AuthenticationReflection.Descriptor.MessageTypes[0]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public AppUser() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public AppUser(AppUser other) : this() {
      idAppUser_ = other.idAppUser_;
      name_ = other.name_;
      surname_ = other.surname_;
      su_ = other.su_;
      email_ = other.email_;
      password_ = other.password_;
      deactivated_ = other.deactivated_;
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public AppUser Clone() {
      return new AppUser(this);
    }

    /// <summary>Field number for the "idAppUser" field.</summary>
    public const int IdAppUserFieldNumber = 1;
    private double idAppUser_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public double IdAppUser {
      get { return idAppUser_; }
      set {
        idAppUser_ = value;
      }
    }

    /// <summary>Field number for the "name" field.</summary>
    public const int NameFieldNumber = 2;
    private string name_ = "";
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public string Name {
      get { return name_; }
      set {
        name_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    /// <summary>Field number for the "surname" field.</summary>
    public const int SurnameFieldNumber = 3;
    private string surname_ = "";
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public string Surname {
      get { return surname_; }
      set {
        surname_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    /// <summary>Field number for the "su" field.</summary>
    public const int SuFieldNumber = 4;
    private bool su_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Su {
      get { return su_; }
      set {
        su_ = value;
      }
    }

    /// <summary>Field number for the "email" field.</summary>
    public const int EmailFieldNumber = 5;
    private string email_ = "";
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public string Email {
      get { return email_; }
      set {
        email_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    /// <summary>Field number for the "password" field.</summary>
    public const int PasswordFieldNumber = 6;
    private string password_ = "";
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public string Password {
      get { return password_; }
      set {
        password_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    /// <summary>Field number for the "deactivated" field.</summary>
    public const int DeactivatedFieldNumber = 7;
    private bool deactivated_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Deactivated {
      get { return deactivated_; }
      set {
        deactivated_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as AppUser);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(AppUser other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (!pbc::ProtobufEqualityComparers.BitwiseDoubleEqualityComparer.Equals(IdAppUser, other.IdAppUser)) return false;
      if (Name != other.Name) return false;
      if (Surname != other.Surname) return false;
      if (Su != other.Su) return false;
      if (Email != other.Email) return false;
      if (Password != other.Password) return false;
      if (Deactivated != other.Deactivated) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      if (IdAppUser != 0D) hash ^= pbc::ProtobufEqualityComparers.BitwiseDoubleEqualityComparer.GetHashCode(IdAppUser);
      if (Name.Length != 0) hash ^= Name.GetHashCode();
      if (Surname.Length != 0) hash ^= Surname.GetHashCode();
      if (Su != false) hash ^= Su.GetHashCode();
      if (Email.Length != 0) hash ^= Email.GetHashCode();
      if (Password.Length != 0) hash ^= Password.GetHashCode();
      if (Deactivated != false) hash ^= Deactivated.GetHashCode();
      if (_unknownFields != null) {
        hash ^= _unknownFields.GetHashCode();
      }
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (IdAppUser != 0D) {
        output.WriteRawTag(9);
        output.WriteDouble(IdAppUser);
      }
      if (Name.Length != 0) {
        output.WriteRawTag(18);
        output.WriteString(Name);
      }
      if (Surname.Length != 0) {
        output.WriteRawTag(26);
        output.WriteString(Surname);
      }
      if (Su != false) {
        output.WriteRawTag(32);
        output.WriteBool(Su);
      }
      if (Email.Length != 0) {
        output.WriteRawTag(42);
        output.WriteString(Email);
      }
      if (Password.Length != 0) {
        output.WriteRawTag(50);
        output.WriteString(Password);
      }
      if (Deactivated != false) {
        output.WriteRawTag(56);
        output.WriteBool(Deactivated);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (IdAppUser != 0D) {
        size += 1 + 8;
      }
      if (Name.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(Name);
      }
      if (Surname.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(Surname);
      }
      if (Su != false) {
        size += 1 + 1;
      }
      if (Email.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(Email);
      }
      if (Password.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(Password);
      }
      if (Deactivated != false) {
        size += 1 + 1;
      }
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(AppUser other) {
      if (other == null) {
        return;
      }
      if (other.IdAppUser != 0D) {
        IdAppUser = other.IdAppUser;
      }
      if (other.Name.Length != 0) {
        Name = other.Name;
      }
      if (other.Surname.Length != 0) {
        Surname = other.Surname;
      }
      if (other.Su != false) {
        Su = other.Su;
      }
      if (other.Email.Length != 0) {
        Email = other.Email;
      }
      if (other.Password.Length != 0) {
        Password = other.Password;
      }
      if (other.Deactivated != false) {
        Deactivated = other.Deactivated;
      }
      _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(pb::CodedInputStream input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
            break;
          case 9: {
            IdAppUser = input.ReadDouble();
            break;
          }
          case 18: {
            Name = input.ReadString();
            break;
          }
          case 26: {
            Surname = input.ReadString();
            break;
          }
          case 32: {
            Su = input.ReadBool();
            break;
          }
          case 42: {
            Email = input.ReadString();
            break;
          }
          case 50: {
            Password = input.ReadString();
            break;
          }
          case 56: {
            Deactivated = input.ReadBool();
            break;
          }
        }
      }
    }

  }

  public sealed partial class LoginRequest : pb::IMessage<LoginRequest> {
    private static readonly pb::MessageParser<LoginRequest> _parser = new pb::MessageParser<LoginRequest>(() => new LoginRequest());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<LoginRequest> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::PROTR.GRPC.Authentication.AuthenticationReflection.Descriptor.MessageTypes[1]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public LoginRequest() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public LoginRequest(LoginRequest other) : this() {
      email_ = other.email_;
      password_ = other.password_;
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public LoginRequest Clone() {
      return new LoginRequest(this);
    }

    /// <summary>Field number for the "email" field.</summary>
    public const int EmailFieldNumber = 1;
    private string email_ = "";
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public string Email {
      get { return email_; }
      set {
        email_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    /// <summary>Field number for the "password" field.</summary>
    public const int PasswordFieldNumber = 2;
    private string password_ = "";
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public string Password {
      get { return password_; }
      set {
        password_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as LoginRequest);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(LoginRequest other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (Email != other.Email) return false;
      if (Password != other.Password) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      if (Email.Length != 0) hash ^= Email.GetHashCode();
      if (Password.Length != 0) hash ^= Password.GetHashCode();
      if (_unknownFields != null) {
        hash ^= _unknownFields.GetHashCode();
      }
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (Email.Length != 0) {
        output.WriteRawTag(10);
        output.WriteString(Email);
      }
      if (Password.Length != 0) {
        output.WriteRawTag(18);
        output.WriteString(Password);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (Email.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(Email);
      }
      if (Password.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(Password);
      }
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(LoginRequest other) {
      if (other == null) {
        return;
      }
      if (other.Email.Length != 0) {
        Email = other.Email;
      }
      if (other.Password.Length != 0) {
        Password = other.Password;
      }
      _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(pb::CodedInputStream input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
            break;
          case 10: {
            Email = input.ReadString();
            break;
          }
          case 18: {
            Password = input.ReadString();
            break;
          }
        }
      }
    }

  }

  #endregion

}

#endregion Designer generated code