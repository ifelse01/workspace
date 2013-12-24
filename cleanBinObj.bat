@echo off 
setlocal enabledelayedexpansion   
color 2f 
rem mode con: cols=80 lines=25 

Set filterPath="%cd%\.\bin" "%cd%\.\obj" "%cd%\bin\SmallSharpTools\Packer for .NET\.\bin"
				
@REM 
@echo cleaning bin/obj folders, please wait...... 
@rem Cycle to delete the current directory and all subdirectories under the SVN file
@rem for /r . %%a in (.) do @if exist "%%a\.svn" @echo "%%a\.svn" 
for /r . %%a in (.) do (		
	Set bFilter=false
	Set bFilterObj=false
		
	if exist "%%a\bin" (
		for %%b in (%filterPath%) do (
			if %%b=="%%a\bin" (
				@echo filtered path"%%a\bin"
				Set bFilter=true
			)
		)

		if "!bFilter!"=="false" (rd /s /q "%%a\bin"  & @echo deleted folder "%%a\bin")
	)  
	if exist "%%a\obj" (
		for %%b in (%filterPath%) do (
			if %%b=="%%a\obj" (
				@echo filtered path："%%a\obj"
				Set bFilterObj=true
			)
		)

		if "!bFilterObj!"=="false" (rd /s /q "%%a\obj"  & @echo deleted folder "%%a\obj")
	)
		
)
@echo Cleaned up!!!
@pause 



