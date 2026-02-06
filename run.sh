#!/bin/bash

cd Mes.Api  

podman-compose -f ./mssql.yaml up -d

# dotnet run

dotnet watch run
