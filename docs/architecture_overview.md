
# Architecture overview

The solution is divided into 3 projects:

- `pbn-lib` - the main project, contains the logic
- `pbn` - the command line interface with command line parsing and application logic
- `pbn-test` - all the tests


## pbn-lib

The most important classes are:

`PbnFile` - contains all the information about a PBN file, including tokens
`PbnParser` - parses a PBN file and returns a `PbnFile`

These classes are located in the root namespace `pbn`.

The library is structured into namespaces each of which contains a separate feature:

### Namespace `pbn.tokens`

Contains all the token classes along with the baseclass for a token called `PbnSemanticToken`.

Contains a sub-namespace `pbn.tokens.tags` which contains some of the predefined tags.

The tokens namespace also contains the `TagFactory` class which is used to create tags.

Basic types of tokens are:
- Tag
- Table tag - tag with table values
- Commentary - a comment
- Escaped line - also the predefined escaped lines % EXPORT and % PBN <version>
- Unrecognized text line - represents a line that is not recognized as any of the above

### Namespace `pbn.model`

Contains all the data classes and enums.

### Namespace `pbn.serialization`

Contains all the serializers for `PbnFile`. Currently only serialization to Pbn is supported.

### Namespace `pbn.manipulators`

Contains all the manipulators for `PbnFile`. There are following manipulators:

- `PbnFileStripper` - strips a file of unnecessary parts
- `PbnFileAnalyzer` - adds analysis to a file
- `PbnInfoPrinter` - prints info about a file
- `PbnBoardManipulator` - facilitates renumbering and removal of boards

### Namespace `pbn.service`

Contains all the services used by the library. Currently only the `IAnalysisService` is used.

### Namespace `pbn.dds`

Implements `IAnalysisService` using the DDS library.

### Namespace `pbn.utils`

Contains debug and list utility classes.

## pbn (cli)

The `pbn` project contains the command line interface.

The application operates on the following bases:

1. Parse a file.
2. Execute all manipulations in a meaningful order, ie:
    - strip file
    - delete boards
    - renumber boards
    - add analysis
3. Serialize the file to a file or to stdout.


For more details, see Code reference or code.