#!/bin/bash

ping_url="https://api.rosette.com/rest/v1"
retcode=0

#------------------ Functions ----------------------------------------------------

#Gets called when the user doesn't provide any args
function HELP {
    echo -e "\nusage: --API_KEY API_KEY [--FILENAME filename] [--ALT_URL altUrl]"
    echo "  API_KEY       - Rosette API key (required)"
    echo "  FILENAME      - C# source file"
    echo "  ALT_URL       - Alternate service URL (optional)"
    echo "Compiles and runs the source file using the published rosette-api from NuGet"
    exit 1
}

#Checks if Rosette API key is valid
function checkAPI() {
    echo "Check API Key"
    match=$(curl "${ping_url}/ping" -H "X-RosetteAPI-Key: ${API_KEY}" |  grep -o "forbidden")
    if [ ! -z $match ]; then
        echo -e "\nInvalid Rosette API Key"
        exit 1
    fi  
}

function cleanURL() {
    echo "Clean URL"
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
    echo "Validate URL"
    match=$(curl "${ping_url}/ping" -H "X-RosetteAPI-Key: ${API_KEY}" -H "user_key: ${API_KEY}" |  grep -o "Rosette API")
    if [ "${match}" = "" ]; then
        echo -e "\n${ping_url} server not responding\n"
        exit 1
    fi  
}

function runExample() {
    echo "Run Example(s)"
    result=""
    echo -e "\n---------- ${1} start -------------"
    executable=$(basename "${1}" .cs).exe
    mcs ${1} -r:rosette_api.dll -r:System.Net.Http.dll -r:System.Web.Extensions.dll -out:$executable 2>&1
    result="$(mono $executable ${API_KEY} ${ALT_URL})"
    if [[ ${result} == *"Exception"* ]]; then
        retcode=1
    fi
    echo ${result}
    echo "---------- ${1} end -------------"
}

#------------------ Functions End ------------------------------------------------
#Gets API_KEY, FILENAME and ALT_URL if present
while getopts ":API_KEY:FILENAME:ALT_URL:GIT_USERNAME:VERSION" arg; do
    case "${arg}" in
        API_KEY)
            API_KEY=${OPTARG}
            usage
            ;;
        ALT_URL)
            ALT_URL=${OPTARG}
            usage
            ;;
        FILENAME)
            FILENAME=${OPTARG}
            usage
            ;;
    esac
done

cleanURL

validateURL

#Copy the mounted content in /source to current WORKDIR
cp -r -n /source/. .

#Run the examples
if [ ! -z ${API_KEY} ]; then
    #Check API key and if succesful then build local rosette_api project
    checkAPI
    #Change to dir where examples will be run from
    pushd rosette_apiExamples
    if [ ! -z ${FILENAME} ]; then
        runExample ${FILENAME}
    else
        for file in *.cs; do
            runExample ${file}
        done
    fi
    popd
else 
    HELP
fi

exit ${retcode}
