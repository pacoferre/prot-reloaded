set PROTOC=%UserProfile%\.nuget\packages\Google.Protobuf.Tools\3.5.1\tools\windows_x64\protoc.exe
set PLUGIN=%UserProfile%\.nuget\packages\Grpc.Tools\1.12.0\tools\windows_x64\grpc_csharp_plugin.exe

%PROTOC% -I=./protos --csharp_out . ./protos/authentication.proto --grpc_out . --plugin=protoc-gen-grpc=%PLUGIN%

endlocal
