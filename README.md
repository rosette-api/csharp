[![Build Status](https://travis-ci.org/rosette-api/csharp.svg?branch=master)](https://travis-ci.org/rosette-api/csharp) [![NuGet version](https://badge.fury.io/nu/rosette_api.svg)](https://badge.fury.io/nu/rosette_api)

## Rosette API
The Rosette Text Analytics Platform uses natural language processing, statistical modeling, and machine learning to
analyze unstructured and semi-structured text across 364 language-encoding-script combinations, revealing valuable
information and actionable data. Rosette provides endpoints for extracting entities and relationships, translating and
comparing the similarity of names, categorizing and adding linguistic tags to text and more.

## Rosette API Access
- Rosette Cloud [Sign Up](https://developer.rosette.com/signup)
- Rosette Enterprise [Evaluation](https://www.rosette.com/product-eval/)

## Quick Start

#### Installation
If you use Nuget, you may use either Visual Studio's Nuget package manager or command line to install the Rosette API binding.

In Visual Studio: search for the [rosette_api package](https://www.nuget.org/packages/rosette_api/) in Nuget and follow the installation instructions.

Using Nuget Command Line: `nuget install rosette_api`.

To check your installed version:

`nuget list rosette_api`

#### Examples
View small example programs for each Rosette endpoint
in the [examples](https://github.com/rosette-api/csharp/tree/develop/rosette_apiExamples) directory.

#### Documentation & Support
- [Binding API](https://rosette-api.github.io/csharp/)
- [Rosette Platform API](https://developer.rosette.com/features-and-functions)
- [Binding Release Notes](https://github.com/rosette-api/csharp/wiki/Release-Notes)
- [Rosette Platform Release Notes](https://support.rosette.com/hc/en-us/articles/360018354971-Release-Notes)
- [Binding/Rosette Platform Compatibility](https://developer.rosette.com/features-and-functions?csharp#)
- [Support](https://support.rosette.com)
- [Binding License: Apache 2.0](https://github.com/rosette-api/csharp/blob/develop/LICENSE.txt)

## Concurrency
The C# binding uses HttpClient to manage connectivity and concurrency.  By default, .NET sets the default connection limit to 2, which is the same as the Rosette API default limit.  For Rosette API plans that allow for higher concurrency, the internal HTTP client will adjust automatically to the higher number.  If a user chooses to provide their own HTTP client, no adjustment will be made.  In this case it is up to the user to set `ServicePointManager.DefaultConnectionLimit` to the Rosette API concurrency level prior to instantiating the CAPI object.

For multithreaded operations, do not instantiate a new CAPI object for each thread.  The objects will not share the connection limit and `429 too many requests` errors are likely to occur. Rather, so that the underlying HttpClient can manage the queueing of the requests across all threads, instantiate a CAPI object and pass it to each thread.  If it is necessary to instantiate a CAPI object on each thread, first create an HttpClient object, either by retrieving it from an instance of CAPI via the `Client` property or by creating your own HTTP client and passing it into each thread for use by the CAPI constructor.

#### Example of using a common CAPI object for each thread:
```
    static void Main(string[] args) {
        TestConcurrency().GetAwaiter().GetResult();
    }

    private static async Task TestConcurrency() {
        int threads = 5;
        var tasks = new List<Task>();
        CAPI api = new CAPI("rosette api key");
        foreach (int task in Enumerable.Range(0, threads)) {
            Console.WriteLine("Starting task {0}", task);
            tasks.Add(Task.Factory.StartNew( () => runLookup(task, api) ));
        }
        await Task.WhenAll(tasks);
        Console.WriteLine("Test complete");
    }

    private static Task runLookup(int taskId, CAPI api) {
        int calls = 5;
        string contentUri = "http://www.foxsports.com/olympics/story/chad-le-clos-showed-why-you-never-talk-trash-to-michael-phelps-080916";
        for (int i = 0; i < calls; i++) {
            Console.WriteLine("Task ID: {0} call {1}", taskId, i);
            try {
                var result = api.Entity(contentUri: contentUri);
                Console.WriteLine("Concurrency: {0}, Result: {1}", api.Concurrency, result);
            }
            catch (Exception ex) {
                Console.WriteLine(ex);
            }
        }
        return Task.CompletedTask;
    }
```

#### Example of retrieving a client object and using it in each thread:
```
    static void Main(string[] args) {
        TestConcurrency().GetAwaiter().GetResult();
    }

    private static async Task TestConcurrency() {
        int threads = 5;
        var tasks = new List<Task>();

        CAPI api = new CAPI("rosette api key");
        HttpClient client = api.Client;
        foreach (int task in Enumerable.Range(0, threads)) {
            Console.WriteLine("Starting task {0}", task);
            tasks.Add(Task.Factory.StartNew( () => runLookup(task, client) ));
        }
        await Task.WhenAll(tasks);
        Console.WriteLine("Test complete");
    }

    private static Task runLookup(int taskId, HttpClient client) {
        int calls = 5;
        string contentUri = "http://www.foxsports.com/olympics/story/chad-le-clos-showed-why-you-never-talk-trash-to-michael-phelps-080916";
        for (int i = 0; i < calls; i++) {
            Console.WriteLine("Task ID: {0} call {1}", taskId, i);
            try {
                CAPI api = new CAPI("rosette api key", client: client);

                var result = api.Entity(contentUri: contentUri);
                Console.WriteLine("Concurrency: {0}, Result: {1}", api.Concurrency, result);
            }
            catch (Exception ex) {
                Console.WriteLine(ex);
            }
        }
        return Task.CompletedTask;
    }
```




## Binding Developer Information
If you are modifying the binding code, please refer to the [developer README](https://github.com/rosette-api/csharp/tree/develop/DEVELOPER.md) file.
