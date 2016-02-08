FROM mono
MAINTAINER Fiona Hasanaj

RUN apt-get -y update && apt-get install -y vim && apt-get install curl
RUN mkdir csharp
WORKDIR csharp
COPY compile-and-run.sh compile-and-run.sh
COPY rosette_api.sln rosette_api.sln

VOLUME ["/source"]

CMD ./compile-and-run.sh $API_KEY $FILENAME $ALT_URL