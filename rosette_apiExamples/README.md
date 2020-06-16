.Net(C#) Examples
=============================

## Endpoint Examples ##
These examples are scripts that can be run independently to demonstrate the Rosette API functionality.

Each example file demonstrates one of the capabilities of the Rosette Platform. Each example, when run, prints its output to the console.

Here are some methods for running the examples.  Each example will also accept an optional `--url` parameter for
overriding the default URL.

A note on prerequisites.  Rosette API only supports TLS 1.2 so ensure your toolchain also supports it.


### Docker Mono image
- install Docker per your OS
- cd to the docker directory under the examples
- `[sudo] docker build -t basistech/mono:1.1 .`
- cd to the examples directory
- Run the example as `[sudo] docker run -e FILENAME=_example_.cs -e API_KEY=_your_api_key_ -v "full-path-to-examples-source:/source" basistech/mono:1.1`. This will compile and run the example using a Mono environment.  If you would like to run against an alternate URL, add `-e ALT_URL=_alternate_url_` before the `-v`.

### Visual Studio
- If you are using Visual Studio, you can use the Nuget Package Manager.  Search for rosette_api in the Online Packages and install.
- If you are using Nuget Command line: `nuget install rosette_api`

You can now run your desired endpoint file to see it in action.

### Command line
- `nuget install rosette_api`
- copy the rosette_api.lib to the same directory as your examples (found in rosette_api.*/)
- Compile the file using `csc _endpoint_.cs /r:rosette_api.dll /r:System.Net.Http.dll /r:System.Web.Extensions.dll`. This will output an .exe file with the _endpoint_ name.

### Running the compiled example
- Run the file using `_endpoint_.exe your_api_key [alternate_url]`

