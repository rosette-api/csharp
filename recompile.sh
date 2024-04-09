#!/bin/bash

# step into the directory of the script
cd "$(dirname "$0")"


# compile the project
nuget restore rosette_api.sln
msbuild /p:Configuration=Release rosette_api.sln
# if last command is empty then exit
if [ $? -ne 0 ]; then
    exit 1
fi

cp packages/Newtonsoft.Json.13.0.2/lib/net45/Newtonsoft.Json.dll rosette_apiExamples/.
cp rosette_api/bin/Release/rosette_api.dll rosette_apiExamples/.
cd rosette_apiExamples
# Loop over parameters
for filename in "$@"
do
  if [ -f "${filename}.cs" ]; then
    echo "####> Compiling and running ${filename} example"  
    csc "${filename}.cs" /r:rosette_api.dll /r:System.Net.Http.dll /r:System.Web.Extensions.dll /r:Newtonsoft.Json.dll
    # if last command is empty then exit
    if [ $? -eq 0 ]; then
        mono "${filename}.exe"
    fi
  else
    echo "####> File ${filename}.cs not found in rosette_apiExamples directory"  
  fi
done