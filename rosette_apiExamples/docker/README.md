---
# Mono Image for CSharp Examples
---
### Summary
To simplify the running of the C# examples, the Dockerfile will build an Mono image and install the latest rosette-api library from NuGet.

### Basic Usage
Build the docker image, e.g. `sudo docker build -t basistech/mono:1.1 .`

Run an example as `sudo docker run -e FILENAME=source-file.cs -e API_KEY=api-key -v "path-to-example-source:/source" basistech/mono:1.1`

To run all of the examples in a directory, from the source directory:

`find -maxdepth 1 -name "*.cs" -print -exec sudo run -e FILENAME={} -e API_KEY=api-key -v "path-to-example-source:/source" basistech/mono:1.1 \;`