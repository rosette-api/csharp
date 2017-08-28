using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using rosette_api;

namespace rosette_apiExamples
{
public class ConcurrencyTest {
        private static int threads = 3;
        private static int calls = 5;

        static void Main(string[] args) {
            // To use the C# API, you must provide an API key
            string apikey = "Your API key";
            string alturl = string.Empty;

            // You may set the API key via command line argument:
            // concurrencyTest yourapikeyhere
            if (args.Length != 0) {
                apikey = args[0];
                alturl = args.Length > 1 ? args[1] : string.Empty;
            }
            // Block on the test, otherwise the threads will exit before completion when main exits
            TestConcurrency(apikey, alturl).GetAwaiter().GetResult();
        }
        private static async Task TestConcurrency(string apikey, string alturl) {
            var tasks = new List<Task>();
            CAPI api = string.IsNullOrEmpty(alturl) ? new CAPI(apikey) : new CAPI(apikey, alturl);
            foreach (int task in Enumerable.Range(0, threads)) {
                Console.WriteLine("Starting task {0}", task);
                tasks.Add(Task.Factory.StartNew( () => runLookup(task, api) ));
            }
            await Task.WhenAll(tasks);
            Console.WriteLine("Test complete");
        }
        private static Task runLookup(int taskId, CAPI api) {
            string contentUri = "http://www.foxsports.com/olympics/story/chad-le-clos-showed-why-you-never-talk-trash-to-michael-phelps-080916";
            foreach (int call in Enumerable.Range(0, calls)) {
                Console.WriteLine("Task ID: {0} call {1}", taskId, call);
                try {
                    var result = api.Entity(contentUri: contentUri);
                    Console.WriteLine("Concurrency: {0},Rresult: {1}", api.Concurrency, result);
                }
                catch (Exception ex) {
                    Console.WriteLine(ex);
                }
            }
            return Task.CompletedTask;
        }
    }
}