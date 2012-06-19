watch('Source\/BlueCollar\.Dashboard\/Static\/Src\/Img\/.*\.png') { system 'MSBuild BlueCollar.targets /t:Img' }
watch('Source\/BlueCollar\.Dashboard\/Static\/Src\/Js\/.*\.js') { system 'MSBuild BlueCollar.targets /t:Js' }
watch('Source\/BlueCollar\.Dashboard\/Static\/Src\/Less\/.*\.less|Bootstrap\/less\/.*\.less') { system 'MSBuild BlueCollar.targets /t:Less' }