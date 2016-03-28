FROM mono
MAINTAINER Chris Park <cpark@basistech.net>
ENV REFRESHED_AT 2016-01-29

RUN mkdir csharp-dev && mkdir csharp-dev/rosette_apiExamples
WORKDIR csharp-dev
RUN nuget install rosette_api && cp rosette_api.*/lib/net45/rosette_api.dll ./rosette_apiExamples
ENV ALT_URL ""
COPY compile-and-run.sh compile-and-run.sh

VOLUME ["/source"]

CMD ./compile-and-run.sh $API_KEY $FILENAME $ALT_URL

