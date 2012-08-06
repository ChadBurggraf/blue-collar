build: bootstrap
	jekyll --no-server --no-auto

bootstrap:
	rm -rf bootstrap/bootstrap
	cd bootstrap; make bootstrap
	cp bootstrap/bootstrap/css/bootstrap.min.css src/css/bootstrap.css
	cp bootstrap/bootstrap/css/bootstrap-responsive.min.css src/css/bootstrap-responsive.css
	cp bootstrap/bootstrap/img/*.png src/img
	cp bootstrap/bootstrap/js/bootstrap.min.js src/js/bootstrap.js
	rm -rf bootstrap/bootstrap

.PHONY: build bootstrap