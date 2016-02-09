---
# Mono Image for CSharp Examples
---
### Summary
To simplify the running of the C# examples, the Dockerfile will build a Mono image and install rosette-api from the local development source.

### Basic Usage
Build the docker image, e.g. `sudo docker build -t basistech/mono:1.1 .`

Run an example as `sudo docker run -e FILENAME=source-file.cs -e API_KEY=api-key -v "path-to-local-csharp-dir:/source" basistech/mono:1.1`

To test against a specific source file, add `-e FILENAME=filename` before the `-v`

Also, to test against an alternate url, add `-e ALT_URL=alternate_url` before the `-v`
