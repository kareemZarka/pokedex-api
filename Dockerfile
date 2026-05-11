# syntax=docker/dockerfile:1           

# ---------- Build stage ----------
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build                                                                                                                                                                                                                             
WORKDIR /src                                                                                                                                                                                                                                                                

# Copy project files first for better layer caching on dependency restore.                                                                                                                                                                                                  
COPY Pokedex.sln ./                                               
COPY src/Pokedex.Api/Pokedex.Api.csproj src/Pokedex.Api/                                                                                                                                                                                                                    
COPY tests/Pokedex.Api.UnitTests/Pokedex.Api.UnitTests.csproj tests/Pokedex.Api.UnitTests/                                                                                                                                                                                  
RUN dotnet restore src/Pokedex.Api/Pokedex.Api.csproj                                                                                                                                                                                                                       

# Copy the rest of the source and publish a self-contained release build.                                                                                                                                                                                                   
COPY . .                                                          
RUN dotnet publish src/Pokedex.Api/Pokedex.Api.csproj \                                                                                                                                                                                                                     
    -c Release \                                                                                                                                                                                                                                                            
    -o /app/publish \                  
    --no-restore                                                                                                                                                                                                                                                            

# ---------- Runtime stage ----------  
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app                                                                                                                                                                                                                                                                
COPY --from=build /app/publish .

EXPOSE 5000                                                       
ENV ASPNETCORE_URLS=http://+:5000      

ENTRYPOINT ["dotnet", "Pokedex.Api.dll"]