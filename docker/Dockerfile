FROM mono:4.0.1
MAINTAINER Fiona Hasanaj

RUN apt-get -y update && apt-get install -y vim && apt-get install curl && apt-get install -y git && apt-get install -y doxygen
RUN mkdir csharp-dev
WORKDIR csharp-dev
COPY compile-and-run.sh compile-and-run.sh
RUN chmod 755 compile-and-run.sh
COPY rosette_api.sln rosette_api.sln

VOLUME ["/source"]

CMD ./compile-and-run.sh $API_KEY $FILENAME $ALT_URL $GIT_USERNAME $VERSION