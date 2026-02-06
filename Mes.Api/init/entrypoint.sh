#!/bin/bash
set -e

echo "Starting SQL Server..."
/opt/mssql/bin/sqlservr &

echo "Waiting for SQL Server to be ready..."
sleep 20

echo "Running init.sql..."
/opt/mssql-tools/bin/sqlcmd \
  -S localhost \
  -U sa \
  -P "$SA_PASSWORD" \
  -i /init/init.sql

wait
