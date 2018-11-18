#!/bin/bash
dotnet test ${TRAVIS_BUILD_DIR}/tests/Users.Test
dotnet test ${TRAVIS_BUILD_DIR}/tests/Permissions.Test
dotnet test ${TRAVIS_BUILD_DIR}/tests/Groups.Test
dotnet test ${TRAVIS_BUILD_DIR}/tests/Posts.Test
dotnet test ${TRAVIS_BUILD_DIR}/tests/SocialNetwork.Test