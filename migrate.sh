#!/usr/bin/env bash
cd AspirePostgresRag.ApiService || exit 1
dotnet ef migrations add $1 --project ../AspirePostgresRag.Data/AspirePostgresRag.Data.csproj 