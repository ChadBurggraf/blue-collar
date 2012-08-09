build: bootstrap
	cat css/src/bootstrap.css css/src/bootstrap-responsive.css css/src/syntax.css css/src/collar.css | java -jar yuicompressor-2.4.7.jar --type css > css/app.css
	cat js/src/jquery.js js/src/bootstrap.js js/src/collar.js | python compile-js.py > js/app.js
	jekyll --no-server --no-auto

cat-assets:
	cat css/src/bootstrap.css css/src/bootstrap-responsive.css css/src/syntax.css css/src/collar.css > css/app.css
	cat js/src/jquery.js js/src/bootstrap.js js/src/collar.js > js/app.js

bootstrap:
	rm -rf bootstrap/bootstrap
	cd bootstrap; make bootstrap
	cp bootstrap/bootstrap/css/bootstrap.min.css css/bootstrap.css
	cp bootstrap/bootstrap/css/bootstrap-responsive.min.css css/bootstrap-responsive.css
	cp bootstrap/bootstrap/img/*.png img
	cp bootstrap/bootstrap/js/bootstrap.min.js js/bootstrap.js
	rm -rf bootstrap/bootstrap

jekyll:
	jekyll --no-server --no-auto

watch:
	watchr -e "watch('css/src/.*\.css|js/src/.*\.js') { system 'make cat-assets' }"

.PHONY: build cat-assets bootstrap jekyll watch