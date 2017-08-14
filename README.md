[![Build Status](https://travis-ci.org/rosette-api/csharp.svg?branch=master)](https://travis-ci.org/rosette-api/csharp)

## .Net (C#) client binding for the Rosette API

## Installation

If you use Nuget, you may use either Visual Studio's Nuget package manager or command line to install the Rosette API binding.

In Visual Studio: search for the [rosette_api package](https://www.nuget.org/packages/rosette_api/) in Nuget and follow the installation instructions.

Using Nuget Command Line: `nuget install rosette_api`.

If the version you are using is not [the latest from nuget](https://www.nuget.org/packages/rosette_api/),
please check for its [**compatibilty with api.rosette.com**](https://developer.rosette.com/features-and-functions?csharp).
If you have an on-premise version of Rosette API server, please contact support for
binding compatibility with your installation.

To check your installed version:

`nuget list rosette_api`

## Docker
A Docker image for running the examples against the compiled source library is available on Docker Hub.

Command: `docker run -e API_KEY=api-key -v "path-to-local-csharp-dir:/source" rosetteapi/docker-csharp`

Additional environment settings:
`-e ALT_URL=<alternative URL>`
`-e FILENAME=<single filename>`

## Basic Usage

See [examples](rosette_apiExamples)

## Concurrency
The C# binding uses HttpClient to manage connectivity and concurrency.  By default, .NET sets the default connection limit to 2, which is the same as the Rosette API default limit.  For Rosette API plans that allow for higher concurrency, the internal HTTP client will adjust automatically to the higher number.  If a user chooses to provide their own HTTP client, no adjustment will be made.  In this case it is up to the user to set `ServicePointManager.DefaultConnectionLimit` to the Rosette API concurrency level prior to instantiating the CAPI object.

For multithreaded operations, do not instantiate a new CAPI object for each thread.  The objects will not share the connection limit and `429 too many requests` errors are likely to occur. Rather, so that the underlying HttpClient can manage the queueing of the requests across all threads, instantiate a CAPI object and pass it to each thread.  If it is necessary to instantiate a CAPI object on each thread, first create an HttpClient object, either by retrieving it from an instance of CAPI via the `Client` property or by creating your own HTTP client and passing it into each thread for use by the CAPI constructor.

#### Example of using a common CAPI object for each thread:
```
    public class ConcurrencyTest {
        [Test]
        public async Task TestConcurrency() {
            var tasks = new List<Task>();

            CAPI api = new CAPI("rosette api key");
            foreach (int task in Enumerable.Range(0, 5)) {
                tasks.Add(Task.Factory.StartNew( () => runLookup(task, client) ));
            }
            await Task.WhenAll(tasks);
        }
        private async Task runLookup(int taskId, CAPI api) {
            string contentUri = "http://www.foxsports.com/olympics/story/chad-le-clos-showed-why-you-never-talk-trash-to-michael-phelps-080916";
            for (int i = 0; i < 100; i++) {
                System.Diagnostics.Debug.WriteLine("Task ID: {0} call {1}", taskId, i);
                try {
                    var result = api.Entity(contentUri: contentUri);
                    System.Diagnostics.Debug.WriteLine("Concurrency: {0},Rresult: {1}", api.Concurrency, result);
                }
                catch (Exception ex) {
                    System.Diagnostics.Debug.WriteLine(ex);
                }
            }
        }
    }
```

#### Example of retrieving a client object and using it in each thread:
```
    public class ConcurrencyTest {
        [Test]
        public async Task TestConcurrency() {
            var tasks = new List<Task>();

            CAPI api = new CAPI("rosette api key");
            HttpClient client = api.Client;
            foreach (int task in Enumerable.Range(0, 5)) {
                tasks.Add(Task.Factory.StartNew( () => runLookup(task, client) ));
            }
            await Task.WhenAll(tasks);
        }
        private async Task runLookup(int taskId, HttpClient client) {
            string contentUri = "http://www.foxsports.com/olympics/story/chad-le-clos-showed-why-you-never-talk-trash-to-michael-phelps-080916";
            for (int i = 0; i < 100; i++) {
                System.Diagnostics.Debug.WriteLine("Task ID: {0} call {1}", taskId, i);
                try {
                    CAPI api = new CAPI("rosette api key", client: client);

                    var result = api.Entity(contentUri: contentUri);
                    System.Diagnostics.Debug.WriteLine("Concurrency: {0},Rresult: {1}", api.Concurrency, result);
                }
                catch (Exception ex) {
                    System.Diagnostics.Debug.WriteLine(ex);
                }
            }
        }
    }
```

## API DocumentationV
See [documentation](http://rosette-api.github.io/csharp)

## Release Notes
Visit the [wik](|https://github.com/rosette-api/csharp/wiki/Release-Notes)

## Additional Information
Visit [Rosette API site](https://developer.rosette.com)
