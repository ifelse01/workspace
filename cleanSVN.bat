@echo on 
color 2f 
mode con: cols=80 lines=25 
@REM 
@echo cleaning SVN files, please wait...... 
@rem Cycle to delete the current directory and all subdirectories under the SVN file
@rem for /r . %%a in (.) do @if exist "%%a\.svn" @echo "%%a\.svn" 
@for /r . %%a in (.) do @if exist "%%a\.svn" rd /s /q "%%a\.svn" 
@echo Cleaned up!!!
@pause 