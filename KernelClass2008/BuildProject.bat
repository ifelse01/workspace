@echo off

Set BuildConfig=Debug
Set vsExe="C:\Program Files\Microsoft Visual Studio 9.0\Common7\IDE\devenv.exe"

set time1=%time:~0,2%%time:~3,2%%time:~6,2%%time:~9,2%

if not exist %vsExe%  (
	Set vsExe="C:\Program Files (x86)\Microsoft Visual Studio 9.0\Common7\IDE\devenv.exe"
)

@echo Start building KernelClass2008.csproj
%vsExe% /clean %BuildConfig% "KernelClass2008.csproj" /Out "BuildProject.%BuildConfig%.log"
%vsExe% /rebuild %BuildConfig% "KernelClass2008.csproj" /Out "BuildProject.%BuildConfig%.log"
@echo End building KernelClass2008.csproj

@echo Copy to SupportDLL
copy D:\workspaces\KernelClass2008\bin\Debug\KernelClass2008.dll D:\workspaces\SupportDLL\

set time2=%time:~0,2%%time:~3,2%%time:~6,2%%time:~9,2%
set /a time3=%time2%-%time1%

echo.

@echo total run time: %time3%ms

echo.

pause & exit
