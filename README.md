[![Build Status](https://travis-ci.org/rosette-api/csharp.svg?branch=master)](https://travis-ci.org/rosette-api/csharp)

.Net (C#) client binding for the Rosette API
==================================

Installation
------------

If you use Nuget, you may use either Visual Studio's Nuget package manager or command line to install the Rosette API binding.

In Visual Studio: search for the [rosette_api package](https://www.nuget.org/packages/rosette_api/) in Nuget and follow the installation instructions.

Using Nuget Command Line: `nuget install rosette_api`.

If the version you are using is not [the latest from nuget](https://www.nuget.org/packages/rosette_api/),
please check for its [**compatibilty with api.rosette.com**](https://developer.rosette.com/features-and-functions?csharp).
If you have an on-premise version of Rosette API server, please contact support for
binding compatibility with your installation.

To check your installed version:

`nuget list rosette_api`

Docker
------
A Docker image for running the examples against the compiled source library is available on Docker Hub.

Command: `docker run -e API_KEY=api-key -v "path-to-local-csharp-dir:/source" rosetteapi/docker-csharp`

Additional environment settings:
`-e ALT_URL=<alternative URL>`
`-e FILENAME=<single filename>`

Basic Usage
-----------

See [examples](rosette_apiExamples)

API Documentation
-----------------

See [documentation](http://rosette-api.github.io/csharp)

Additional Information
----------------------

Visit [Rosette API site](https://developer.rosette.com)
