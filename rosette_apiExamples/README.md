## Endpoint Examples
These examples are scripts that can be run independently to demonstrate the Rosette API functionality.

Each example file demonstrates one of the capabilities of the Rosette Platform. Each example, when run, prints its output to the console.

Each example will also accept an optional, alternate url parameter for overriding the default URL.

A note on prerequisites.  Rosette API only supports TLS 1.2 so ensure your toolchain also supports it.

Here are some methods for running the examples.

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
  cp Newtonsoft.Json.10.0.2/lib/net45/Newtonsoft.Json.dll rosette_apiExamples/.
  cp rosette_api.1.14.4/lib/net45/rosette_api.dll rosette_apiExamples/.
  ```
- Compile the example you'd like to execute.  E.g. language.cs
  ```
  cd rosette_apiExamples
  csc language.cs /r:rosette_api.dll /r:System.Net.Http.dll /r:System.Web.Extensions.dll
  ```
- Run the compiled example against Rosette Cloud.  In this example, your Cloud API key is stored in the environment variable `$API_KEY`.
  ```
  mono language.exe $API_KEY
  ```

#### Latest Source with Docker
- Clone the repository.
  ```
  git clone git@github.com:rosette-api/csharp.git
  cd csharp
  ```
- Launch a `mono` container.
  ```
  docker run -it -v $(pwd):/csharp mono:6
  ```
- Build the package from source.
  ```
  cd /csharp
  nuget restore rosette_api.sln
  xbuild /p:Configuration=Release rosette_api.sln
  ```
- _Optional:_ Run the Unit Tests.
  ```
  mono ./packages/NUnit.Console.3.0.1/tools/nunit3-console.exe ./rosette_apiUnitTests/bin/Release/rosette_apiUnitTests.dll
  ```
- Copy the runtime binaries to the examples directory.
  ```
  cp packages/Newtonsoft.Json.10.0.3/lib/net45/Newtonsoft.Json.dll rosette_apiExamples/.
  cp rosette_api/bin/Release/rosette_api.dll rosette_apiExamples/.
  ```
- Compile the example you'd like to execute.  E.g. language.cs
  ```
  cd rosette_apiExamples
  csc language.cs /r:rosette_api.dll /r:System.Net.Http.dll /r:System.Web.Extensions.dll
  ```
- Run the compiled example against Rosette Cloud.  In this example, your Cloud API key is stored in the environment variable `$API_KEY`.
  ```
  mono language.exe $API_KEY
  ```

### Visual Studio
- If you are using Visual Studio, you can use the Nuget Package Manager.  Search for `rosette_api` in the Online Packages and install.

You can now run your desired endpoint file to see it in action.
