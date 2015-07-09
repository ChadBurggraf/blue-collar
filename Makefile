VERSION = 1.6
CSS = css/app-${VERSION}.css
JS = js/app-${VERSION}.js

build: bootstrap
	cat static/css/bootstrap.css static/css/bootstrap-responsive.css static/css/syntax.css static/css/fancybox.css static/css/collar.css | java -jar yuicompressor-2.4.7.jar --type css > ${CSS}
	cat static/js/jquery.js static/js/bootstrap.js static/js/fancybox.js static/js/collar.js | python compile-js.py > ${JS}
	jekyll build

cat-assets:
	cat static/css/bootstrap.css static/css/bootstrap-responsive.css static/css/syntax.css static/css/fancybox.css static/css/collar.css > ${CSS}
	cat static/js/jquery.js static/js/bootstrap.js static/js/fancybox.js static/js/collar.js > ${JS}

bootstrap:
	rm -rf bootstrap/bootstrap
	cd bootstrap; make bootstrap
	cp bootstrap/bootstrap/css/bootstrap.min.css static/css/bootstrap.css
	cp bootstrap/bootstrap/css/bootstrap-responsive.min.css static/css/bootstrap-responsive.css
	cp bootstrap/bootstrap/img/*.png img
	cp bootstrap/bootstrap/js/bootstrap.min.js static/js/bootstrap.js
	rm -rf bootstrap/bootstrap

jekyll:
	jekyll build

watch:
	watchr -e "watch('static/css/.*\.css|static/js/.*\.js') { system 'make cat-assets; echo' }"

.PHONY: build cat-assets bootstrap jekyll watch