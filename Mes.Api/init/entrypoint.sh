#!/bin/bash
set -e

echo "Starting SQL Server..."
/opt/mssql/bin/sqlservr &

echo "Waiting for SQL Server to be ready..."
until /opt/mssql-tools/bin/sqlcmd \
  -S localhost \
  -U sa \
  -P "$MSSQL_SA_PASSWORD" \
  -Q "SELECT 1" > /dev/null 2>&1
do
  echo "⏳ SQL not ready yet..."
  sleep 2
done

echo "Checking if database 'mes' exists..."

DB_EXISTS=$(
  /opt/mssql-tools/bin/sqlcmd \
    -S localhost \
    -U sa \
    -P "$MSSQL_SA_PASSWORD" \
    -Q "SET NOCOUNT ON; SELECT COUNT(*) FROM sys.databases WHERE name = 'mes'" \
    -h -1 -W
)

if [ "$DB_EXISTS" = "0" ]; then
  echo "Database not found. Running init.sql..."
  /opt/mssql-tools/bin/sqlcmd \
    -S localhost \
    -U sa \
    -P "$MSSQL_SA_PASSWORD" \
    -i /init/init.sql
  echo "Init.sql completed."
else
  echo "Database already exists. Skipping init.sql."
fi

wait
