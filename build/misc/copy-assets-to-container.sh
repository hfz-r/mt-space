#!/bin/bash
set -ex

readonly ROOT_DIR=`pwd`
readonly DESTINATION_PATH=${ROOT_DIR}/deploy/k8s/helm/envoy-proxy/assets
readonly ENVOY_CONFIG_PATH=${ROOT_DIR}/src/ApiGateways/Envoy/envoy.yaml
readonly ENVOY_SSL_PATH=${ROOT_DIR}/src/ApiGateways/Envoy/ssl
readonly PROTO_PB_PATH=${ROOT_DIR}/src/Proto/*.pb

echo "#################### Begin copying $ENVOY_CONFIG_PATH into $DESTINATION_PATH ####################"
sudo cp -p $ENVOY_CONFIG_PATH $DESTINATION_PATH

echo "#################### Begin copying $ENVOY_SSL_PATH into $DESTINATION_PATH ####################"
sudo cp -p -r $ENVOY_SSL_PATH $DESTINATION_PATH

echo "#################### Begin copying $PROTO_PB_PATH into $DESTINATION_PATH ####################"
sudo cp -p $PROTO_PB_PATH $DESTINATION_PATH

echo "FINISHED: Assets copied."