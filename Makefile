build:
	mkdir -p Dashboard/css
	mkdir -p Dashboard/img
	mkdir -p Dashboard/js
	cp Bootstrap/bootstrap/css/*.css Dashboard/css
	cp Bootstrap/bootstrap/img/* Dashboard/img
	cp Bootstrap/bootstrap/js/*.js Dashboard/js
	recess --compile Dashboard/less/*.less > Dashboard/css/collar.css

watch:
	watchr -e "watch('Bootstrap/less/*\.less|Dashboard/less/*\.less') { system 'make -C Bootstrap && make bootstrap -C Bootstrap && make' }"