# PBN

PBN is a command line tool for working with Portable Bridge Notation files.

Credit project (zápočtový program) for Cpp Programming course at MFF Cuni.

## Used libraries

PBN utilizes some libraries:

- Boost::program_options

## Supported functions

- parse a file and validate it
- printing overview information about a file
- stripping a file of unnecessary parts

## Features plan

- creating board analyses in a file
- exports to pdf, html
- stricter adherence to PBN standard (currently it is not fully compliant)
  - validate
  - support more obscure import variant
  - check used features against declared version
- pbn generation using external tools (e.g. Bigdeal)

## User manual

User manual can be found in [guide.md](guide.md).

Alternatively, it can be found in the generated documentation under **guide**.

Use `--help` flag, or `-h` to see some of the options.

## Building

PBN is written in C++20 and uses CMake as build system.

### Step 0: Dependencies

To build PBN, you need to configure CMake to be able to find Boost.

To install Boost, see [Boost downloads section](https://www.boost.org/users/download/).

Alternatively, use a package manager or Visual Studio NuGet or a compiler with Boost pre-packaged.

### Step 1: Configure

Create a directory where you want to build and enter it.

Run CMake with the `CMakeLists.txt` file as argument:

```bash
cmake CMakeLists.txt
```

To use a different generator, you can use the `-G` flag:

```bash
cmake CMakeLists.txt -G "MinGW Makefiles"
```

### Step 2: Build

To build, run

```bash
cmake --build <build dir>
```

## Documentation

This project uses Doxygen to create documentation.

To build the documentation, use the provided Doxyfile or create or own and run

```bash
doxygen Doxyfile
```

Documentation will be generated in the `docs` directory.

## Testing

To run the tests, build the project. A second target of pbn_test should be built, simply execute it:

```bash
./pbn_test
```

Additionally, more options can be provided, run with help flag to see.

```bash
./pbn_test --help
```

See [Boost.Test docs](https://www.boost.org/doc/libs/1_80_0/libs/test/doc/html/index.html) for more info.

## Authors

[Zdeněk Tomis](https://zdenektomis.eu)
