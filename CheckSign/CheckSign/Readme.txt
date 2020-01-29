Check Sign : 1.0.0.0
Check Sign App
CheckSign.exe [/Help] /drop Drop Path /expand Package expansion path. [/maxdepth Maximum recursion depth.] [/norecurse] [/powershell] /results Results Path. [/skippackages] [/verbose]

Description:
    This application is made to find unsigned files in a build drop.

Parameter List: (* == Required)
    /drop          * ( /d )    Drop path to be checked. This should be a local copy of the build drop.
    /expand        * ( /e )    Path where package files will be expaneded. Expanding the same place can generate paths that are too deep to process.
    /Help            ( /h  /? ) Default: "False" : Display program usage information.
    /maxdepth        ( /m )    Default: "100" : Maximum recursion depth.
    /norecurse       ( /n )    Default: "False" : This flag is used to prevent checking subdirectories of the drop folder.
    /powershell      ( /p )    Default: "False" : Verifying signatures in *.ps* files.
    /results       * ( /r )    A path where results will be written.
    /skippackages    ( /s )    Default: "False" : Skip Verifying signatures in packages.
    /verbose         ( /v )    Default: "False" : Use more verbose console output.


Example:
Copy the drop files to be checked to a local path like E:\Drops\Test20190101\Files
From the command line execute:
   CheckSign \drop E:\Drops\Test20190101\Files \r E:\Drops\TestSign20200108\Results \e e:\Expand \powershell
   
The process will scan the files and write a set of log files in the results directory (in append mode);
Review the logs

