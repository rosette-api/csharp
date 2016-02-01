FROM mono
MAINTAINER Chris Park <cpark@basistech.net>
ENV REFRESHED_AT 2016-01-29

RUN mkdir rosette
WORKDIR rosette
RUN nuget install rosette_api && cp rosette_api.*/lib/net45/rosette_api.dll .
ENV ALT_URL ""
COPY compile-and-run.sh compile-and-run.sh

VOLUME ["source"]

CMD ./compile-and-run.sh $FILENAME $API_KEY $ALT_URL

