

# filesplitass.exe 
A Cobalt-Strike (CS) compatible execute-assembly console application written to be compatible with .NET Framework 2.x - 4.x and Mono.

Supports the splitting of a single file using CS (execute-assembly) or Windows Command Prompt (on-disk). Additionally Powershell.exe is supported but testing identified quirks so reccommend Command Prompt.
Tested with 100GB file to split to 100 sub files on Window 7, dot net core 2.1.3

Usage:

filesplitass.exe <soure_target> <directory_to_save_subfiles> <max_size_in_bytes]

Example 1: filesplitass.exe "c:\path\file.data"  "c:\subfile_folder" 100000

# Credits:
Code is a heavy modified devriavative of lapth (Lap Tran) filesplit repository. 
