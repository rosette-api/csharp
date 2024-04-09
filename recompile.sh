#!/bin/bash

# step into the directory of the script
cd "$(dirname "$0")"
# exit if $1 is not in rosette_apiExamples directory as a .cs file


# compile the project
nuget restore rosette_api.sln
msbuild /p:Configuration=Release rosette_api.sln

cp packages/Newtonsoft.Json.13.0.2/lib/net45/Newtonsoft.Json.dll rosette_apiExamples/.
cp rosette_api/bin/Release/rosette_api.dll rosette_apiExamples/.
# Loop over parameters
for filename in "$@"
do
  if [ -f "rosette_apiExamples/${filename}.cs" ]; then
    echo "####> Compiling and running ${filename} example"  
    pushd rosette_apiExamples
    csc "${filename}.cs" /r:rosette_api.dll /r:System.Net.Http.dll /r:System.Web.Extensions.dll /r:Newtonsoft.Json.dll

    mono "${filename}.exe"
    popd
  else
    echo "####> File ${filename}.cs not found in rosette_apiExamples directory"  
  fi
done