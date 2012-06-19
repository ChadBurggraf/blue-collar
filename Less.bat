@echo off

if "%1" equ "watch" goto watch

pushd Bootstrap && call make.bat && popd
xcopy Bootstrap\img\* Source\BlueCollar.Dashboard\Static\Src\Img /I /Y /Q > nul
xcopy Bootstrap\js\*.js Source\BlueCollar.Dashboard\Static\Src\Js\Vendor\Bootstrap /I /Y /Q > nul
xcopy Bootstrap\docs\assets\css\*.css Source\BlueCollar.Dashboard\Static\Src\Css /I /Y /Q > nul
del Source\BlueCollar.Dashboard\Static\Src\Css\docs.css
call recess --compile Source\BlueCollar.Dashboard\Static\Src\Less\collar.less > Source\BlueCollar.Dashboard\Static\Src\Css\collar.css
goto :EOF

:watch
echo Watching LESS files...
watchr -e "watch('Source\/BlueCollar\.Dashboard\/Static\/Src\/Less\/.*\.less') { system 'Less.bat' }"