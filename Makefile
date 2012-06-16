build:
	mkdir -p Dashboard/css
	mkdir -p Dashboard/img
	mkdir -p Dashboard/js
	cp Bootstrap/docs/assets/css/bootstrap.css Dashboard/css
	cp Bootstrap/docs/assets/css/bootstrap-responsive.css Dashboard/css
	cp Bootstrap/img/* Dashboard/img
	cp Bootstrap/docs/assets/js/bootstrap.min.js Dashboard/js
	recess --compile Dashboard/less/collar.less > Dashboard/css/collar.css

rebuild:
	make -C Bootstrap
	make build

watch:
	echo "Watching less files..."; \
	watchr -e "watch('Dashboard/less/.*\.less') { system 'make' }"