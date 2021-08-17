#!/bin/bash
set -ex

readonly ROOT_DIR=`pwd`
readonly INCLUDE_PATH=/mnt/c/Users/hafiz.roslan/.nuget/packages/grpc.tools/2.33.1/build/native/include
readonly PROTO_PATH=${ROOT_DIR}/src/Proto
readonly PROTO_FILE=*.proto

protoc -I$INCLUDE_PATH -I$PROTO_PATH \
    --include_imports --include_source_info \
    --descriptor_set_out=$PROTO_PATH/proto.pb \
    $PROTO_PATH/$PROTO_FILE
