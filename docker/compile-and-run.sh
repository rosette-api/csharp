#!/bin/bash -e

#Gets called when the user doesn't provide any args
function HELP {
    echo -e "\nusage: source_file.cs API_KEY [ALT_URL]"
    echo "  API_KEY       - Rosette API key (required)"
    echo "  FILENAME      - C# source file"
    echo "  ALT_URL       - Alternate service URL (optional)"
    echo "  GIT_USERNAME  - Git username where you would like to push regenerated gh-pages (optional)"
    echo "  VERSION       - Build version (optional)"
    echo "Compiles and runs the source file using the local development source"
    exit 1
}

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
        GIT_USERNAME)
            GIT_USERNAME=${OPTARG}
            usage
            ;;
        VERSION)
            VERSION=${OPTARG}
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
    #Check API key and if succesful then build local rosette_api project
    checkAPI && nuget restore rosette_api.sln && xbuild /p:Configuration=Release rosette_api.sln
    #Copy necessary libraries
    cp /csharp-dev/rosette_api/bin/Release/rosette_api.dll /csharp-dev/rosette_apiExamples
    cp /csharp-dev/rosette_apiUnitTests/bin/Release/nunit.framework.dll /csharp-dev/rosette_apiUnitTests
    cp /csharp-dev/rosette_apiUnitTests/bin/Release/rosette_api.dll /csharp-dev/rosette_apiUnitTests
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
    #Change to dir where unit tests will be run from
    cd ../rosette_apiUnitTests
    for file in *.cs; do
            echo -e "\n---------- $file start -------------"
            executable=$(basename "$file" .cs).exe
            mcs $file -r:rosette_api.dll -r:System.Net.Http.dll -r:System.Web.Extensions.dll -r:WindowsBase.dll -r:nunit.framework.dll -out:$executable
            mono $executable 
            echo "---------- $file end -------------"
    done
else 
    HELP
fi

#Generate gh-pages and push them to git account (if git username and version are provided)
if [ ! -z ${GIT_USERNAME} ] && [ ! -z ${VERSION} ]; then
    #clone csharp git repo to the root dir
    cd /
    git clone git@github.com:${GIT_USERNAME}/csharp.git
    cd csharp
    git checkout origin/gh-pages -b gh-pages
    git branch -d develop
    #generate gh-pages from development source and output the contents to csharp repo
    cd /csharp-dev
    mv /csharp-dev/rosette_api /csharp-dev/rosette_api_dox
    #configure doxygen
    doxygen -g rosette_api
    sed -i '/^\bPROJECT_NAME\b/c\PROJECT_NAME = "rosette_api"' rosette_api
    sed -i "/^\bPROJECT_NUMBER\b/c\PROJECT_NUMBER = $VERSION" rosette_api
    sed -i '/^\bOPTIMIZE_OUTPUT_JAVA\b/c\OPTIMIZE_OUTPUT_JAVA = YES' rosette_api
    sed -i '/^\bEXTRACT_ALL\b/c\EXTRACT_ALL = YES' rosette_api
    sed -i '/^\bEXTRACT_STATIC\b/c\EXTRACT_STATIC = YES' rosette_api
    sed -i '/^\bUML_LOOK\b/c\UML_LOOK = YES' rosette_api
    sed -i '/^\bHAVE_GRAPH\b/c\HAVE_GRAPH = YES' rosette_api
    sed -i '/^\bGENERATE_LATEX\b/c\GENERATE_LATEX = NO' rosette_api
    sed -i '/^\bGENERATE_HTML\b/c\GENERATE_HTML = YES' rosette_api
    sed -i '/^\bINPUT\b/c\INPUT = ./rosette_api_dox' rosette_api
    sed -i '/^\bRECURSIVE\b/c\RECURSIVE = YES' rosette_api
    sed -i '/^\bFILE_PATTERNS\b/c\FILE_PATTERNS = *.c *.cc *.cxx *.cpp *.c++ *.java *.ii *.ixx *.ipp *.i++ *.inl *.h *.hh *.hxx *.hpp *.h++ *.idl *.odl *.cs *.php *.php3 *.inc *.m *.mm *.py *.f90' rosette_api
    sed -i '/^\bOUTPUT_DIRECTORY\b/c\OUTPUT_DIRECTORY = /csharp' rosette_api
    sed -i '/^\bHTML_OUTPUT\b/c\HTML_OUTPUT = html' rosette_api
    #generate docs
    doxygen rosette_api
    cd /csharp
    git add .
    git commit -a -m "publish csharp apidocs ${VERSION}"
    git push
fi