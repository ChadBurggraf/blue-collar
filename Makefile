build: bootstrap
	jekyll --no-server --no-auto

bootstrap:
	rm -rf bootstrap/bootstrap
	cd bootstrap; make bootstrap
	cp bootstrap/bootstrap/css/bootstrap.min.css css/bootstrap.css
	cp bootstrap/bootstrap/css/bootstrap-responsive.min.css css/bootstrap-responsive.css
	cp bootstrap/bootstrap/img/*.png img
	cp bootstrap/bootstrap/js/bootstrap.min.js js/bootstrap.js
	rm -rf bootstrap/bootstrap

.PHONY: build bootstrap