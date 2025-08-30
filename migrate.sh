#!/usr/bin/env bash
cd VibeCatch.ApiService || exit 1
dotnet ef migrations add $1 --project ../VibeCatch.Data/VibeCatch.Data.csproj 