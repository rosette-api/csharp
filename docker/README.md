---
# Mono Image for CSharp Examples
---
### Summary
To simplify the running of the C# examples, the Dockerfile will build a Mono image and install rosette-api from the local development source.

### Basic Usage
Build the docker image, e.g. `sudo docker build -t basistech/mono:1.1 .`

Run an example as `sudo docker run -e FILENAME=source-file.cs -e API_KEY=api-key -v "path-to-local-csharp-dir:/source" basistech/mono:1.1`

To test against a specific source file, add `-e FILENAME=filename` before the `-v`, to test against an alternate url, add `-e ALT_URL=alternate_url`, and optionally if you would like to regenerate gh-pages from the changes made to the development source you can add `-e GIT_USERNAME=git-username -e VERSION=version` before the `-v`. In order to push the gh-pages to git remember to mount .ssh and .gitconfig to the root dir `-v path-to-.ssh-dir:/root/.ssh -v path-to-.gitconfig:/root/.gitconfig`.
