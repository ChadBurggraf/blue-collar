@echo off
MSBuild BlueCollar.targets /t:LessBootstrap
echo Watching Bootstrap LESS files...
watchr -e "watch('Bootstrap\/less\/.*\.less') { system 'MSBuild BlueCollar.targets /t:LessBootstrap' }"