#!/bin/bash

cd Mes.Api  
podman-compose -f ./mssql.yaml up
dotnet run