
# User guide

To use this project, run the pbn executable;

## Options reference

### -h, [ --help ]       
Produces a help message with all the options listed

### --version          
print version information

### -v [ --verbose ]                      
print additional information about the file

### -s [ --strip ]                        
Removes all results, site and event information.

### -o [ --output ] arg  
Output file name, if not specified, the
program will use the input file name

If no output file is specified, nor the `-w` flag is used, the program will print the file to stdout.

This option can be specified as the second positional option.

### -w [ --overwrite ] 
Overwrite the input file with the output file.

If no output file is specified, nor the `-w` flag is used, the program will print the file to stdout.

### --info
Print information about the file to standard output

### --inputfile (required)
Specify the input pbn file. This option is mandatory. 

This option can be specified as the first positional option.