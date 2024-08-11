cmake_minimum_required(VERSION 3.14)
project(pbn)

if(POLICY CMP0144)
    cmake_policy(SET CMP0144 NEW)
endif()

# Set the default build type if not specified
if(NOT CMAKE_BUILD_TYPE)
  set(CMAKE_BUILD_TYPE Release)
endif()

set(CMAKE_VERBOSE_MAKEFILE ON)
set(PUBLISH_DIR "${CMAKE_BINARY_DIR}/publish")
set(PUBLISH_CONFIG "Debug")
set(BOOST_ROOT /opt/homebrew/Cellar/boost/1.85.0)


# Optionally configure platform-specific flags or settings
if(CMAKE_SYSTEM_NAME STREQUAL "Linux")
  set(MAKEFILE "Makefile_linux_shared")
elseif(CMAKE_SYSTEM_NAME STREQUAL "Darwin")
  set(MAKEFILE "Makefile_Mac_clang_shared")
endif()

# Find Boost
find_package(Boost 1.56 REQUIRED COMPONENTS thread CONFIG)

if(Boost_FOUND)
    set(CC_BOOST ${BOOST_ROOT})
    include_directories(${Boost_INCLUDE_DIRS})
    add_definitions(-DBOOST_ALL_NO_LIB)
else()
    message(FATAL_ERROR "Could not find Boost libraries")
endif()


# Add the DDS library as an external project
include(ExternalProject)
ExternalProject_Add(dds
    # Define where to download the project from
    GIT_REPOSITORY "https://github.com/zdenecek/dds.git"
    GIT_TAG "develop"  # You can specify a branch, tag, or commit

    # Directory to store the source and build
    PREFIX "${CMAKE_BINARY_DIR}/dds"

    # Configure, build, and install commands
    CONFIGURE_COMMAND ""
    BUILD_COMMAND bash -c "cd src && make -f Makefiles/${MAKEFILE} CC_BOOST=${CC_BOOST} &&\
    shopt -s nullglob &&\
    mkdir -p ${CMAKE_BINARY_DIR}/pbn-lib &&\
    cp *.so *.a *.dll ${CMAKE_BINARY_DIR}/pbn-lib &&\
    cp *.so *.a *.dll ${CMAKE_BINARY_DIR}/dds &&\
    shopt -u nullglob"
    BUILD_IN_SOURCE 1  # Optional: Use if building in the source directory is required
    INSTALL_COMMAND ""
    TEST_COMMAND ""
)


set(ENV{DDS_LIB_PATH} "${CMAKE_BINARY_DIR}/dds")

# Detect the current platform and set the RID accordingly
if(CMAKE_SYSTEM_NAME STREQUAL "Windows")
    if(CMAKE_SIZEOF_VOID_P EQUAL 8)
        set(RUNTIME_IDENTIFIER "win-x64")
    else()
        set(RUNTIME_IDENTIFIER "win-x86")
    endif()
elseif(CMAKE_SYSTEM_NAME STREQUAL "Darwin")
    if(CMAKE_SYSTEM_PROCESSOR STREQUAL "arm64")
        set(RUNTIME_IDENTIFIER "osx-arm64")
    else()
        set(RUNTIME_IDENTIFIER "osx-x64")
    endif()
elseif(CMAKE_SYSTEM_NAME STREQUAL "Linux")
    if(CMAKE_SIZEOF_VOID_P EQUAL 8)
        set(RUNTIME_IDENTIFIER "linux-x64")
    else()
        set(RUNTIME_IDENTIFIER "linux-x86")
    endif()
else()
    message(FATAL_ERROR "Unsupported platform")
endif()

# Copy the .NET project to the build directory
add_custom_target(
  CopyDotnetProject ALL
  COMMAND ${CMAKE_COMMAND} -E copy_directory "${CMAKE_SOURCE_DIR}/pbn" "${CMAKE_BINARY_DIR}/pbn"
  COMMAND ${CMAKE_COMMAND} -E copy_directory "${CMAKE_SOURCE_DIR}/pbn-lib" "${CMAKE_BINARY_DIR}/pbn-lib"
  COMMAND ${CMAKE_COMMAND} -E copy "${CMAKE_SOURCE_DIR}/pbn.sln" "${CMAKE_BINARY_DIR}/pbn.sln"
  COMMENT "Copying .NET project to build directory"
)

# Add custom target for the .NET project to depend on the DDS build
add_custom_target(
  DotnetBuild ALL
  COMMAND dotnet build "${CMAKE_BINARY_DIR}/pbn" -o "${CMAKE_BINARY_DIR}/build"  -r ${RUNTIME_IDENTIFIER} 
  DEPENDS CopyDotnetProject dds  # Ensures that the DDS library is built first
  COMMENT "Building .NET Application"
)

add_custom_target(
  DotnetPublish ALL
  COMMAND dotnet publish "${CMAKE_BINARY_DIR}/pbn" -c ${PUBLISH_CONFIG} -r ${RUNTIME_IDENTIFIER} --self-contained true -o "${PUBLISH_DIR}"
  DEPENDS CopyDotnetProject dds  # Ensures that the DDS library is built first
  COMMENT "Publishing .NET Application"
)

