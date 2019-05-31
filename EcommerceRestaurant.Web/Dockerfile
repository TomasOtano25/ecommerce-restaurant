FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY ["EcommerceRestaurant.Web/EcommerceRestaurant.Web.csproj", "EcommerceRestaurant.Web/"]
COPY ["EcommerceRestaurant.Common/EcommerceRestaurant.Common.csproj", "EcommerceRestaurant.Common/"]
RUN dotnet restore "EcommerceRestaurant.Web/EcommerceRestaurant.Web.csproj"
COPY . .
WORKDIR "/src/EcommerceRestaurant.Web"
RUN dotnet build "EcommerceRestaurant.Web.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "EcommerceRestaurant.Web.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "EcommerceRestaurant.Web.dll"]