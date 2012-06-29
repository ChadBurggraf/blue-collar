/**
 * Manages the view for system-wide notices.
 *
 * @constructor
 */
var NoticeView = Backbone.View.extend({
    className: 'alert alert-block alert-notice',
    events: {
        "click a.close": "destroy"
    },
    id: 'notice',
    tagName: 'div',
    template: _.template($('#notice-template').html()),

    /**
     * Initialization.
     *
     * @param {Object} options Initialization options.
     */
    initialize: function(options) {
        this.options = _.extend({
            margin: 40
        }, options);

        this.scroll();
        $(window).on('scroll', _.bind(this.scroll, this));
    },

    /**
     * Destroys the view singleton.
     */
    destroy: function() {
        NoticeView.destroy();
    },

    /**
     * Renders the view.
     *
     * @return {NaviItemView} This instance.
     */
    render: function() {
        this.$el
            .css('display', 'none')
            .html(this.template(this.model.toJSON()));

        return this;
    },

    /**
     * Handles the window's scroll event.
     */
    scroll: function() {
        var margin = this.options.margin - $(window).scrollTop();

        if (margin > 0) {
            this.$el.css('marginTop', margin + 'px');
        } else {
            this.$el.css('marginTop', '0');
        }
    }
});

/**
 * Static functions.
 */
_.extend(NoticeView, {
    destroying: false,
    timeout: null,

    /**
     * Creates or replaces the singleon {NoticeView}.
     *
     * @param {Object} options A set of display options.
     * @return {NoticeView} The created or replaced {NoticeView} instance.
     */
    create: function(options) {
        var el = $('#notice'),
            className,
            view;

        options = _.extend({
            destroy: true,
            margin: 40,
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
        el = view.$el;

        if (className) {
            el.addClass(className);
        }

        $('body').append(el);
        view.scroll();
        el.fadeIn();

        if (options.destroy) {
            if (NoticeView.timeout) {
                clearTimeout(NoticeView.timeout);
            }

            NoticeView.timeout = setTimeout(NoticeView.destroy, options.timeout);
        }

        return view;
    },

    /**
     * Destroys the singleton {NoticeView} if it exists and is not
     * already being destroyed.
     */
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