@echo off
MSBuild BlueCollar.targets /t:Js;Less
echo Watching JS and LESS files...
watchr BlueCollar.watchr