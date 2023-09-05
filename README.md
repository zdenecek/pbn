# PBN

PBN is a command line tool for working with Portable Bridge Notation files.


## Supported functions

- parse a file and validate it
- printing overview information about a file
- stripping a file of unnecessary parts
- add analysis to a file

## Features plan

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

PBN is written in .NET 7 and C#.

###  Build

To build, run

```bash
cmake --build <build dir>
```

## Documentation

This project uses dotnet to create documentation.

To build the documentation, run

```bash
```

Documentation will be generated in the `g` directory.

## Testing

To run the tests, run

```bash
dotnet test
```

## Authors

[Zdeněk Tomis](https://zdenektomis.eu)
