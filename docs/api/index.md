# PBN Command line utility

To use this project, run the pbn-cli executable.

The program accepts the following options and arguments:

####  --help
Produces a help message with all the options listed

#### --version
print version information

#### -v [ --verbose ]
print additional information about the file

#### input-file (1st position argument) (required)
Specify the input pbn file. This option is mandatory.

This option can be specified as the first positional option.

#### output-file (2nd position argument)
Specify the output pbn file. This option is optional.

If no output file is specified, nor the `-w` flag is used, the program will print the file to stdout.

#### -r, [--renumber] arg
Renumber boards, use +/-x to shift numbers, x to assign new numbers

Examples:
- -r +3 will shift all board numbers by 3
- -r 3 will assign board numbers starting from 3 (3, 4, 5, ...)
- -r -3 will shift all board numbers by -3

#### -d, [--delete-boards]
Delete boards, accepts numbers or number ranges

Examples:
- -d 3 will delete board 3
- -d 3,5 will delete boards 3 and 5
- -d 3-5 will delete boards 3, 4 and 5
- -d 3,5-7 will delete boards 3, 5, 6 and 7


#### -s [ --strip ]
Removes all results, site and event information.

#### -o [ --output ] arg
Output file name, if not specified, the
program will use the input file name

If no output file is specified, nor the `-w` flag is used, the program will print the file to stdout.

This option can be specified as the second positional option.

#### -w [ --overwrite ]
Overwrite the input file with the output file.

If no output-file is specified, nor the `-w` flag is used, the program will print the file to stdout.

#### --info
Print information about the file to standard output

