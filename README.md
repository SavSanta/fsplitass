
# filesplitass.exe 
A Cobalt-Strike (CS) compatible execute-assembly console application written to be compatible with .NET Framework 2.x - 4.x and Mono.

Supports the splitting of a single file using CS (execute-assembly) or Windows Command Prompt (on-disk). Additionally Powershell.exe is supported but testing identified quirks under certain circumstances due to the way that terminal processes command input so recommend utilization of Windows Command Prompt if dropping on-disk.
Tested with 15GB file split to 5 sub files on Window 10/Windows 11 running NET 4.0.

Usage:

filesplitass.exe <soure_target> <directory_to_save_subfiles> <max_size_in_bytes]

Example 1: filesplitass.exe "c:\path\file.data"  "c:\subfile_folder" 100000
Example 2: filesplitass.exe "c:\backup\largefile.bin"  "subfile_folder" 5000000000


# Compilation
1. Load the Solution in Visual Studio
2. Build Solution.

# Credits:
Code is a heavy modified devriavative of lapth (Lap Tran) filesplit repository. 
