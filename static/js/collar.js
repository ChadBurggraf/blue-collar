(function($) {
    var Sidebar,
        Subnav,
        Window = $(window);

    /* Sidebar pseudo fixed position. */

    Sidebar = function(el, options) {
        this.el = el;
        this.height = el.outerHeight(true);
        this.scrollTop = Window.scrollTop();
        this.winHeight = Window.height();
        this.winWidth = Window.width();

        this.options = $.extend({
            minWindowWidth: 768
        }, options);

        this.position();

        Window.on('resize', $.proxy(this.resize, this));
        Window.on('scroll', $.proxy(this.scroll, this));
    };

    Sidebar.prototype.position = function() {
        if (this.winWidth >= this.options.minWindowWidth) {
            if (this.scrollTop > 0) {
                if (this.winHeight > this.height) {
                    this.el.css('top', this.scrollTop + 'px');
                } else if (this.scrollTop + this.winHeight > this.height) {
                    this.el.css('top', (this.scrollTop + this.winHeight - this.height) + 'px');
                }
            } else {
                this.el.css('top', '');
            }
        } else {
            this.el.css('top', '');
        }
    };

    Sidebar.prototype.resize = function() {
        this.winHeight = Window.height();
        this.winWidth = Window.width();
        this.position();
    };

    Sidebar.prototype.scroll = function() {
        this.scrollTop = Window.scrollTop();
        this.position();
    };

    /* Subnav pseudo fixed position. */

    Subnav = function(el, options) {
        this.el = el;
        this.top = el.length > 0 ? el.offset().top : 0;
        this.parent = el.parent();
        this.isFixed = false;
        
        this.options = $.extend({
            fixedClass: 'subnav-fixed',
            scrollTopOffset: 60
        }, options);


        this.scroll();

        this.el.on('click', $.proxy(this.click, this));
        Window.on('resize', $.proxy(this.resize, this));
        Window.on('scroll', $.proxy(this.scroll, this));
    };

    Subnav.prototype.click = function() {
        var offset = this.options.scrollTopOffset;

        if (!this.isFixed) {
            setTimeout(function () {  
                Window.scrollTop(Window.scrollTop() - offset); 
            }, 10);
        }
    };

    Subnav.prototype.resize = function() {
        if (this.isFixed && this.parent.length > 0) {
            this.el.css('left', this.parent.offset().left + 'px');
        }
    };

    Subnav.prototype.scroll = function() {
        var scrollTop = Window.scrollTop();

        if (scrollTop >= this.top && !this.isFixed) {
            this.isFixed = true;
            this.el.addClass(this.options.fixedClass);
        } else if (scrollTop <= this.top && this.isFixed) {
            this.isFixed = false;
            this.el.removeClass(this.options.fixedClass);
            this.el.css('left', '');
        }

        this.resize();
    };

    new Sidebar($('.sidebar'));
    new Subnav($('.subnav'));
})(jQuery);