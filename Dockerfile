# Optionally, Comment this section to build the api locally

FROM mcr.microsoft.com/dotnet/sdk:6.0 as BUILDER
WORKDIR /srv/api
COPY ./ /srv/
# When building locally, these two commands build the server
RUN dotnet test
RUN dotnet build -c Release

# Build the API Container
FROM mcr.microsoft.com/dotnet/aspnet:6.0
COPY --from=BUILDER /srv/api/bin/Release/net6.0/ /srv/uah-fee-calc/
# Use this copy when using building the api locally
#COPY ./api/bin/Release/net6.0/ /srv/uah-fee-calc/
COPY ./api/UAHFeeSchedule.xlsx /srv/uah-fee-calc/
WORKDIR /srv/uah-fee-calc
ENTRYPOINT dotnet /srv/uah-fee-calc/api.dll