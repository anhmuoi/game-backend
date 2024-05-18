FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS runtime
WORKDIR /app

# install dependencies
RUN apt-get update \
    && apt-get install -y libgdiplus libc6-dev

# Setting the LD_LIBRARY_PATH environment variable so the systems dynamic linker can find the newly installed libraries.
ENV LD_LIBRARY_PATH="/app"
