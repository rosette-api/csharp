.Net(C#) Examples
==================

These examples are can be run independently to demonstrate the Rosette API functionality.

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

Each example, when run, prints its output to the console.

| File Name                     | What it does                                          |
| -------------                 |-------------                                        |
| categories.cs                    | Gets the category of a document at a URL              |
| entities.cs                      | Gets the entities from a piece of text                |
| info.cs                          | Gets information about Rosette API                    |
| language.cs                      | Gets the language of a piece of text                  |
| morphology_complete.cs               | Gets the complete morphological analysis of a piece of text|
| morphology_compound-components.cs    | Gets the de-compounded words from a piece of text     |
| morphology_han-readings.cs           | Gets the Chinese words from a piece of text           |
| morphology_lemmas.cs                 | Gets the lemmas of words from a piece of text         |
| morphology_parts-of-speech.cs        | Gets the part-of-speech tags for words in a piece of text |
| name_deduplication.cs | Deduplicates a list of names |
| name_similarity.cs                  | Gets the similarity score of two names                |
| name_translation.cs               | Translates a name from one language to another        |
| ping.cs                          | Pings the Rosette API to check for reachability       |
| sentences.cs                     | Gets the sentences from a piece of text               |
| sentiment.cs                     | Gets the sentiment of a local file                    |
| tokens.cs                        | Gets the tokens (words) from a piece of text          |
| topics.cs | Gets the key phrases and concepts from a piece of text |
| transliteration.cs | Transliterates a name |

