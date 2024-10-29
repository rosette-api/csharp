#!/bin/bash

if [ $# -eq 0 ]; then
    echo "Usage: $0 API_KEY [ALT_URL]" 1>&2
    exit 1
fi

filenames=$( ls rosette_apiExamples/*.cs | awk -F/ '{print $NF}' | awk -F. '{print $1}' )

export API_KEY=$1
export ALT_URL=$2

bash recompile.sh $filenames