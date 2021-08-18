#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.
ARG new_relic_license_key
ARG new_relic_app_name

FROM mcr.microsoft.com/dotnet/aspnet:5.0-focal AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0-focal AS build
WORKDIR /src
COPY ["photography-gallery.csproj", ""]
RUN dotnet restore "./photography-gallery.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "photography-gallery.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "photography-gallery.csproj" -c Release -o /app/publish

FROM base AS final

# Install the New Relic APM agent
RUN apt-get update && apt-get install -y wget ca-certificates gnupg \
&& echo 'deb http://apt.newrelic.com/debian/ newrelic non-free' | tee /etc/apt/sources.list.d/newrelic.list \
&& wget https://download.newrelic.com/548C16BF.gpg \
&& apt-key add 548C16BF.gpg \
&& apt-get update \
&& apt-get install -y newrelic-netcore20-agent

# Enable the New Relic APM agent
ENV CORECLR_ENABLE_PROFILING=1 \
CORECLR_PROFILER={36032161-FFC0-4B61-B559-F6C5D41BAE5A} \
CORECLR_NEWRELIC_HOME=/usr/local/newrelic-netcore20-agent \
CORECLR_PROFILER_PATH=/usr/local/newrelic-netcore20-agent/libNewRelicProfiler.so \
NEW_RELIC_LICENSE_KEY=$new_relic_key \
NEW_RELIC_APP_NAME=$new_relic_app_name

WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "photography-gallery.dll"]