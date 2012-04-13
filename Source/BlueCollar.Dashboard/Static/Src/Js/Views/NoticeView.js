/**
 * View implementation for global notices.
 */
var NoticeView = Backbone.View.extend({
    className: 'alert-message block-message',
    events: {
        "click a.close": "destroy"
    },
    id: 'notice',
    tagName: 'div',
    template: _.template($('#notice-template').html()),

    initialize: function(options) {
        this.resize();
        $(window).resize(_.bind(this.resize, this));
    },

    destroy: function() {
        NoticeView.destroy();
    },

    render: function() {
        $(this.el)
            .css('display', 'none')
            .html(this.template(this.model.toJSON()));

        return this;
    },

    resize: function() {
        var el = $(this.el),
            sub = el.outerWidth() - el.width(),
            main = $('#main'),
            mainWidth = main.outerWidth(),
            mainOffset = main.offset();
        
        el.css({
            width: (mainWidth - sub) + 'px',
            left: mainOffset.left + 'px'
        });
    }
});

// Static NoticeView functions. Provides
// singleton NoticeView creation and destruction.
_.extend(NoticeView, {
    destroying: false,
    timeout: null,

    create: function(options) {
        var el = $('#notice'),
            className,
            view;

        options = _.extend({
            destroy: true,
            scroll: true,
            timeout: 7500
        }, options);

        if (el.length > 0 && !NoticeView.destroying) {
            el.remove();
        }

        if (options.className) {
            className = options.className;
            delete options.className;
        }

        if (options.model && !(options.model instanceof Backbone.Model)) {
            options.model = new Backbone.Model(options.model);
        } else if (!options.model) {
            options.model = new Backbone.Model();
        }

        view = new NoticeView(options).render();
        el = $(view.el);

        if (className) {
            el.addClass(className);
        }

        $('body').append(el);
        view.resize();
        el.fadeIn();

        if (options.scroll) {
            scrollTo(0, 0);
        }

        if (options.destroy) {
            if (NoticeView.timeout) {
                clearTimeout(NoticeView.timeout);
            }

            NoticeView.timeout = setTimeout(NoticeView.destroy, options.timeout);
        }

        return view;
    },

    destroy: function() {
        var el = $('#notice');

        if (el.length > 0) {
            NoticeView.destroying = true;

            el.fadeOut(function() {
                el.remove();
                NoticeView.destroying = false;
            });
        }
    }
});