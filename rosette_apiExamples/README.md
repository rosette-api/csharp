## Endpoint Examples
These examples are scripts that can be run independently to demonstrate the Babel Street Analytics API functionality.

Each example file demonstrates one of the capabilities of the Analytics Platform. Each example, when run, prints its output to the console.

Each example will also accept an optional, alternate url parameter for overriding the default URL.

A note on prerequisites.  Analytics API only supports TLS 1.2 so ensure your toolchain also supports it.

Here are some methods for running the examples.

#### Latest Source with Docker
- Clone the repository.
  ```
  git clone git@github.com:rosette-api/csharp.git
  cd csharp
  ```
- Launch a container.
  ```
  docker run -it -v $(pwd):/csharp debian:13
  ```
- Set up the environment.
  ```
  apt-get update
  apt-get install -y wget libicu76

  wget https://dot.net/v1/dotnet-install.sh -O dotnet-install.sh && chmod +x dotnet-install.sh
  ./dotnet-install.sh --version latest
  export DOTNET_ROOT=/root/.dotnet
  export PATH="$PATH:$DOTNET_ROOT:$DOTNET_ROOT/tools"
  ```

- _Optional:_ Setup environment for running unit tests with nunit console.  You can run them without this.
  ```
  apt-get install -y unzip
  wget https://github.com/nunit/nunit-console/releases/download/3.22.0/NUnit.Console-3.22.0.zip
  unzip NUnit.Console-3.22.0.zip

  ```
- Build the package from source.
  ```
  cd /csharp
  dotnet restore rosette_api.slnx
  dotnet build /p:Configuration=Release rosette_api.slnx
  dotnet build /p:Configuration=Debug rosette_api.slnx
  ```
- _Optional:_ Run the Unit Tests.
  ```
  dotnet test ./rosette_apiUnitTests/bin/Release/net10.0/rosette_apiUnitTests.dll
  ```

  Or with the nunit console runner...
  ```
  dotnet /root/bin/net8.0/nunit3-console.dll ./rosette_apiUnitTests/bin/Release/net10.0/rosette_apiUnitTests.dll
  ```
- Prepare a project for the example you'd like to execute.  E.g. language.cs
  ```
  cd rosette_apiExamples
  mkdir LanguageExample
  cd LanguageExample
  dotnet new console --framework net10.0
  cp ../Language.cs ./Program.cs
  dotnet add reference ../../rosette_api/rosette_api.csproj
  ```
- Run the example against Analytics Cloud.  In this example, your Cloud API key is stored in the environment variable `$API_KEY`.
  ```
  dotnet run $API_KEY
  ```
  Or against an alternate url.  The key, in this case, can be anything if you aren't using authorization
  ```
  dotnet run $API_KEY http://example.com:8181/rest/v1
  ```


#### TODO:  Refresh After Publish
#### Latest Version on NuGet with Docker
- Clone the repository.
  ```
  git clone git@github.com:rosette-api/csharp.git
  cd csharp
  ```
- Launch a `mono` container.
  ```
  docker run -it -v $(pwd):/csharp mono:6
  ```
- Install the package from NuGet
  ```
  cd /csharp
  nuget install rosette_api
  ```
- Copy the runtime binaries to the examples directory.
  ```
  cp Newtonsoft.Json.13.0.2/lib/net45/Newtonsoft.Json.dll rosette_apiExamples/.
  cp rosette_api.1.14.4/lib/net45/rosette_api.dll rosette_apiExamples/.
  ```
- Compile the example you'd like to execute.  E.g. language.cs
  ```
  cd rosette_apiExamples
  csc language.cs /r:rosette_api.dll /r:System.Net.Http.dll /r:System.Web.Extensions.dll
  ```
- Run the compiled example against Analytics Cloud.  In this example, your Cloud API key is stored in the environment variable `$API_KEY`.
  ```
  mono language.exe $API_KEY
  ```

