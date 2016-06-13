#!/bin/bash -e

ping_url="https://api.rosette.com/rest/v1"
retcode=0
errors=( "Exception" "processingFailure" "badRequest" "ParseError" "ValueError" "SyntaxError" "AttributeError" "ImportError" )

#------------------ Functions ----------------------------------------------------

#Gets called when the user doesn't provide any args
function HELP {
    echo -e "\nusage: --API_KEY API_KEY [--FILENAME filename] [--ALT_URL altUrl]"
    echo "  API_KEY       - Rosette API key (required)"
    echo "  FILENAME      - C# source file"
    echo "  ALT_URL       - Alternate service URL (optional)"
    echo "Compiles and runs the source file using the local development source"
    exit 1
}

if [ ! -z ${ALT_URL} ]; then
    ping_url=${ALT_URL}
fi

#Checks if Rosette API key is valid
function checkAPI {
    match=$(curl "${ping_url}/ping" -H "X-RosetteAPI-Key: ${API_KEY}" |  grep -o "forbidden")
    if [ ! -z $match ]; then
        echo -e "\nInvalid Rosette API Key"
        exit 1
    fi  
}

function cleanURL() {
    # strip the trailing slash off of the alt_url if necessary
    if [ ! -z "${ALT_URL}" ]; then
        case ${ALT_URL} in
            */) ALT_URL=${ALT_URL::-1}
                echo "Slash detected"
                ;;
        esac
        ping_url=${ALT_URL}
    fi
}

function validateURL() {
    match=$(curl "${ping_url}/ping" -H "X-RosetteAPI-Key: ${API_KEY}" |  grep -o "Rosette API")
    if [ "${match}" = "" ]; then
        echo -e "\n${ping_url} server not responding\n"
        exit 1
    fi  
}

function runExample() {
    result=""
    echo -e "\n---------- ${1} start -------------"
    executable=$(basename "${1}" .cs).exe
    mcs ${1} -r:rosette_api.dll -r:System.Net.Http.dll -r:System.Web.Extensions.dll -out:$executable
    result="$(mono $executable ${API_KEY} ${ALT_URL})"
    echo "${result}"
    echo "---------- ${1} end -------------"
    for err in "${errors[@]}"; do 
        if [[ ${result} == *"${err}"* ]]; then
            retcode=1
        fi
    done
}

#------------------ Functions End ------------------------------------------------

#Gets API_KEY, FILENAME and ALT_URL if present
while getopts ":API_KEY:FILENAME:ALT_URL" arg; do
    case "${arg}" in
        API_KEY)
            API_KEY=${OPTARG}
            ;;
        ALT_URL)
            ALT_URL=${OPTARG}
            ;;
        FILENAME)
            FILENAME=${OPTARG}
            ;;
    esac
done

cleanURL

validateURL

#Copy the mounted content in /source to current WORKDIR
cp -r -n /source/. .
#Remove the obj and bin directories to force clean compile
rm -rf ./rosette_api/bin
rm -rf ./rosette_api/obj

#Run the examples
if [ ! -z ${API_KEY} ]; then
    #Check API key and if succesful then build local rosette_api project
    checkAPI && nuget restore rosette_api.sln
    xbuild /p:Configuration=Release rosette_api.sln
    xbuild /p:Configuration=Debug rosette_api.sln
    #Copy necessary libraries
    cp /csharp-dev/rosette_api/bin/Release/rosette_api.dll /csharp-dev/rosette_apiExamples
    cp /csharp-dev/rosette_apiUnitTests/bin/Release/nunit.framework.dll /csharp-dev/rosette_apiUnitTests
    cp /csharp-dev/rosette_apiUnitTests/bin/Release/rosette_api.dll /csharp-dev/rosette_apiUnitTests
    #Change to dir where examples will be run from
    pushd rosette_apiExamples
    if [ ! -z ${FILENAME} ]; then
        echo -e "\nRunning example against: ${ping_url}\n"
        runExample ${FILENAME}
    else
        echo -e "\nRunning examples against: ${ping_url}\n"
        for file in *.cs; do
            runExample ${file}
        done
    fi
    # Run the unit tests
    popd
    mono ./packages/NUnit.Console.3.0.1/tools/nunit3-console.exe ./rosette_apiUnitTests/bin/Debug/rosette_apiUnitTests.dll
else 
    HELP
fi

exit ${retcode}

