#!/bin/bash
set -ex

readonly ROOT_DIR=`pwd`
readonly INCLUDE_PATH=/mnt/c/Users/hafiz.roslan/.nuget/packages/grpc.tools/2.33.1/build/native/include
readonly PROTO_PATH=${ROOT_DIR}/src/Proto/openapiv2

api=''
proto_file=''
target_dir=''

while [[ $# -gt 0 ]]; do
    case "$1" in
        -p | --project )
          api="$2"; shift 2;;
        *)
          echo "Unknown option $1"
          usage; exit 2 ;;
    esac
done

if [[ $api == "commission" ]]; then
    echo "Generating OpenAPI spec for Commission API..."
    target_dir=${ROOT_DIR}/src/Services/Commission/Commission.API
    proto_file=investor.proto
fi

protoc -I$INCLUDE_PATH -I$PROTO_PATH \
    --openapiv2_out ${target_dir}/wwwroot \
    --openapiv2_opt logtostderr=true,allow_merge=true \
    $PROTO_PATH/$proto_file
