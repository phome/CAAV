CAAV
====

Console Automatic Assembly Version Increase/Decrease( CAAV )


CAAV ( Console Automatic Assembly Version ) automatically increase/decrease the AssemblyInfo version numbers, even in the template* file.

CAAV is a command line tool. The usage is very simple.

caav.exe [start path] [-] *.*.*.*
caav.exe [start path] [-] ...* 

The first sample will automatically increase/decrease four segments of the version. And the second one just increase/decrease the last segment.

CAAV will searh all the AssmblyInfo.cs and AssmblyInfo.tmpl files in the [start path]( If not specified , the path is were the caav.exe belongs to. ) and it's sub-directories and increase/decrease the version number you specified.  

minuse ([-]) is Optional. CAAV do decreasing job if [-] specified. On the other hand , CAAV do increasing job if NOT specified or specified [+] instead.

* Template AssemblyInfo file: eg. We create a template file, if we want to use the SVN version as part of our version. The following is a segments code from the AssemblyInfo.tmpl

// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion("1.0.3.$WCREV$")]
[assembly: AssemblyFileVersion("1.0.3.$WCREV$")] 

CAAV will just increase/decrease the first three segments of the version number in this template file as you specified. But CAAV will not increase/decrease the last segment in this template file , even if you specified.

