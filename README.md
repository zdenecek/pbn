# PBN

PBN is a command line tool for working with [Portable Bridge Notation (`.pbn`) files](https://www.tistis.nl/pbn/).

Some of the supported features include:

- parse a file and validate it
- printing overview information about a file
- stripping a file of unnecessary parts
- add analysis to a file

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

### Third-Party Libraries

This project uses the [`dds` library](https://github.com/dds-bridge/dds), which is licensed under the Apache License 2.0. For more details, see the [LICENSE-APACHE](LICENSE-APACHE) file included in this repository.

For modifications to the `dds` library, see my [dds fork](https://github.com/zdenecek/dds/) repository.

## Features plan

- exports to pdf, html
- stricter adherence to PBN standard (currently it is not fully compliant)
    - validate
    - support more obscure import variant
    - check used features against declared version
- pbn generation using external tools (e.g. Bigdeal)

## User manual

User manual can be found in [guide.md](./guide.md).

Use `--help` flag to see the options.

## Building

PBN is written in .NET Core 7 and C#.

To build on Windows x86, you can use the provided prebuild `dds.dll` file.

```bash
dotnet build
```

Otherwise, you need to compile `dds` from sources. Luckily, this project provides a CMake build script to make it simple for you.

```bash
mkdir build
cd build
cmake ..
make DotnetPublish
```

The executable will be in the `publish` directory.

Should you run into any issues, good luck in CMake hell.

## Documentation

This project uses docfx to create documentation.
You need to have it installed to build the docs.

To build the documentation, run

```bash
docfx docs/docfx.json
```

Documentation will be generated in the `docs/_site` directory.

If you want to serve the documentation locally, run

```bash
docfx docs/docfx.json --serve
```

## Testing

To run the tests, run

```bash
dotnet test
```

## Authors

[Zdeněk Tomis](https://zdenektomis.eu)
