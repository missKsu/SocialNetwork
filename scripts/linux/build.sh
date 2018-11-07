#!/bin/bash
dotnet restore ${TRAVIS_BUILD_DIR}
dotnet build ${TRAVIS_BUILD_DIR}