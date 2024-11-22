<a href="https://www.babelstreet.com/rosette">
<picture>
  <source media="(prefers-color-scheme: light)" srcset="https://charts.babelstreet.com/icon-dark.png">
  <source media="(prefers-color-scheme: dark)" srcset="https://charts.babelstreet.com/icon-light.png">
  <img alt="Babel Street Logo" width="48" height="48">
</picture>
</a>

# Analytics by Babel Street

---

[![NuGet version](https://badge.fury.io/nu/rosette_api.svg)](https://badge.fury.io/nu/rosette_api)

Our product is a full text processing pipeline from data preparation to extracting the most relevant information and
analysis utilizing precise, focused AI that has built-in human understanding. Text Analytics provides foundational
linguistic analysis for identifying languages and relating words. The result is enriched and normalized text for
high-speed search and processing without translation.

Text Analytics extracts events and entities — people, organizations, and places — from unstructured text and adds the
structure of associating those entities into events that deliver only the necessary information for near real-time
decision making. Accompanying tools shorten the process of training AI models to recognize domain-specific events.

The product delivers a multitude of ways to sharpen and expand search results. Semantic similarity expands search
beyond keywords to words with the same meaning, even in other languages. Sentiment analysis and topic extraction help
filter results to what’s relevant.

## Analytics API Access
- Analytics Cloud [Sign Up](https://developer.babelstreet.com/signup)

## Quick Start

#### Installation
If you use Nuget, you may use either Visual Studio's Nuget package manager or command line to install the Analytics API binding.

In Visual Studio: search for the [rosette_api package](https://www.nuget.org/packages/rosette_api/) in Nuget and follow the installation instructions.

Using Nuget Command Line: `nuget install rosette_api`.

To check your installed version:

`nuget list rosette_api`

#### Examples
View small example programs for each Rosette endpoint
in the [examples](https://github.com/rosette-api/csharp/tree/develop/rosette_apiExamples) directory.

#### Documentation & Support
- [Binding API](https://rosette-api.github.io/csharp/)
- [Analytics Platform API](https://docs.babelstreet.com/API/en/index-en.html)
- [Binding Release Notes](https://github.com/rosette-api/csharp/wiki/Release-Notes)
- [Analytics Platform Release Notes](https://docs.babelstreet.com/Release/en/rosette-cloud.html)
- [Support](https://babelstreet.my.site.com/support/s/)
- [Binding License: Apache 2.0](https://github.com/rosette-api/csharp/blob/develop/LICENSE.txt)

## Concurrency
The C# binding uses HttpClient to manage connectivity and concurrency.  By default, .NET sets the default connection 
limit to 2, which is the same as the Analytics API default limit.  For Analytics API plans that allow for higher 
concurrency, the internal HTTP client will adjust automatically to the higher number.  If a user chooses to provide 
their own HTTP client, no adjustment will be made.  In this case it is up to the user to set 
`ServicePointManager.DefaultConnectionLimit` to the Analytics API concurrency level prior to instantiating the CAPI object.

For multithreaded operations, do not instantiate a new CAPI object for each thread.  The objects will not share the 
connection limit and `429 too many requests` errors are likely to occur. Rather, so that the underlying HttpClient can 
manage the queueing of the requests across all threads, instantiate a CAPI object and pass it to each thread.  If it is 
necessary to instantiate a CAPI object on each thread, first create an HttpClient object, either by retrieving it from 
an instance of CAPI via the `Client` property or by creating your own HTTP client and passing it into each thread for use by the CAPI constructor.

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
