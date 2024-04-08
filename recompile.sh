#!/bin/bash

# step into the directory of the script
cd "$(dirname "$0")"
# exit if $1 is not in rosette_apiExamples directory as a .cs file
if [ ! -f "rosette_apiExamples/${1}.cs" ]; then
    echo ".cs not found!"
    exit 1
fi

# compile the project
nuget restore rosette_api.sln
msbuild /p:Configuration=Release rosette_api.sln

cp packages/Newtonsoft.Json.13.0.2/lib/net45/Newtonsoft.Json.dll rosette_apiExamples/.
cp rosette_api/bin/Release/rosette_api.dll rosette_apiExamples/.

cd rosette_apiExamples
csc "${1}.cs" /r:rosette_api.dll /r:System.Net.Http.dll /r:System.Web.Extensions.dll

mono "${1}.exe"