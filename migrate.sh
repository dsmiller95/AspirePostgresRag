#!/usr/bin/env bash
cd ApiService || exit 1
dotnet ef migrations add $1 --project ../Data/Data.csproj 