#!/bin/bash -e

if [ $# -lt 2 ]; then
    echo "usage: $0 source_file.cs api_key [alternate_url]"
    echo "  source_file   - C# source file"
    echo "  api_key       - Rosette API key (required)"
    echo "  alternate_url - Alternate service URL (optional)"
    echo "Compiles and runs the source file using the published rosette-api from NuGet"
    exit 1
fi

source="../source/$1"
executable=$(basename "$1" .cs).exe
alt_url=""

if [ ! -z "$3" ]; then
    alt_url=$3
fi

mcs $source -r:rosette_api.dll -r:System.Net.Http.dll -r:System.Web.Extensions.dll -out:$executable
mono $executable $2 $alt_url
