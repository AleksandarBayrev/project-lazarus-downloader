#!/bin/bash
echo "Cleaning previous build artifacts..."
rm -rfv ./bin
rm -rfv ./obj
echo "Publishing for windows-x64..."
dotnet publish -c Release -r windows-x64 --self-contained false
echo "Publish completed."