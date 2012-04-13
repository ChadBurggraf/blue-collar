:: Watches for CSS and JS changes and re-combines all outputs. Will only
:: update outputs in BlueCollar.Dashboard; a re-compile is necessary to
:: update the static assets embedded in BlueCollar.dll.
Tools\JSPack\JSPack.exe /map:Source\JSPack.xml /actions:false /watch:true