#!/bin/bash

if [ -n $1 ]; then
        cd $1
fi

if [ -d ./artifacts ]; then
        rm -rf ./artifacts
fi

pushd "./test/Printer.Test/"
dotnet test -c Release
popd

dotnet publish "./src/Printer" -c Release -o "../../artifacts"