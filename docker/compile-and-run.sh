#!/bin/bash -e

#Gets called when the user doesn't provide any args
function HELP {
    echo -e "\nusage: source_file.cs API_KEY [ALT_URL]"
    echo "  API_KEY       - Rosette API key (required)"
    echo "  FILENAME      - C# source file"
    echo "  ALT_URL       - Alternate service URL (optional)"
    echo "Compiles and runs the source file using the local development source"
    exit 1
}

#Gets API_KEY, FILENAME and ALT_URL if present
while getopts ":API_KEY:FILENAME:ALT_URL" arg; do
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

#Checks if Rosette API key is valid
function checkAPI {
    match=$(curl "https://api.rosette.com/rest/v1/ping" -H "user_key: ${API_KEY}" |  grep -o "forbidden")
    if [ ! -z $match ]; then
        echo -e "\nInvalid Rosette API Key"
        exit 1
    fi  
}

#Copy the mounted content in /source to current WORKDIR
cp -r -n /source/* .

#Run the examples
if [ ! -z ${API_KEY} ]; then
    #checkAPI
    #Build local rosette_api project
    xbuild /p:Configuration=Release rosette_api.sln
    cp /csharp/rosette_api/bin/Release/rosette_api.dll /csharp/rosette_apiExamples
    #Change to dir where examples will be run from
    cd rosette_apiExamples
    if [ ! -z ${FILENAME} ]; then
        echo -e "\n---------- ${FILENAME} start -------------"
        executable=$(basename "${FILENAME}" .cs).exe
        mcs ${FILENAME} -r:rosette_api.dll -r:System.Net.Http.dll -r:System.Web.Extensions.dll -out:$executable
        mono $executable ${API_KEY} ${ALT_URL}
        echo "---------- ${FILENAME} end -------------"
    else
        for file in *.cs; do
            echo -e "\n---------- $file start -------------"
            executable=$(basename "$file" .cs).exe
            mcs $file -r:rosette_api.dll -r:System.Net.Http.dll -r:System.Web.Extensions.dll -out:$executable
            mono $executable ${API_KEY} ${ALT_URL}
            echo "---------- $file end -------------"
        done
    fi
else 
    HELP
fi