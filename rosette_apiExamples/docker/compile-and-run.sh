#!/bin/bash -e

if [ $# -lt 2 ]; then
    echo "usage: $0 source_file.cs api_key"
    echo "Compiles and runs the source file using the published rosette-api from NuGet"
    exit 1
fi

source="../source/$1"
executable=$(basename "$1" .cs).exe

mcs $source -r:rosette_api.dll -r:System.Net.Http.dll -r:System.Web.Extensions.dll -out:$executable
mono $executable $2
