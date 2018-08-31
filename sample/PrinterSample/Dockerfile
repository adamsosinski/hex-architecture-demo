# publish application
FROM microsoft/dotnet:2.1-sdk AS build-env
WORKDIR /app
COPY ./ ./
RUN dotnet publish ./sample/PrinterSample/PrinterSample.csproj -c Release -o /app/out

# build runtime image
FROM microsoft/dotnet:2.1-runtime 
WORKDIR /app
COPY --from=build-env /app/out ./
ENV NETCORE_ENVIRONMENT=docker
ENTRYPOINT ["dotnet", "PrinterSample.dll"]