@echo off
MSBuild BlueCollar.targets /t:Js;Less;Templates
echo Watching JS, LESS and template files...
watchr BlueCollar.watchr