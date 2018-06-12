set PROTOC=%UserProfile%\.nuget\packages\Google.Protobuf.Tools\3.5.1\tools\windows_x64\protoc.exe
set PLUGIN=%CD%/node_modules/.bin/protoc-gen-ts.cmd
set OUT_DIR=./projects/protr/src/lib/gRPC

%PROTOC% --plugin="protoc-gen-ts=%PLUGIN%" --js_out="import_style=commonjs,binary:%OUT_DIR%" --ts_out="service=true:%OUT_DIR%" -I=../../NETCore/PROTR/gRPC/protos authentication.proto