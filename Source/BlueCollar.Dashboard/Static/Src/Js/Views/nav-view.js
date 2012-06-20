/**
 * Manages the view for a single navigation item.
 *
 * @constructor
 */
var NavItemView = Backbone.View.extend({
    tagName: 'li',
    template: _.template($('#nav-item-template').html()),

    /**
     * Initialization.
     *
     * @param {Object} options Initialization options.
     */
    initialize: function(options) {
        this.model.bind('change', this.render, this);
    },

    /**
     * Renders the view.
     *
     * @return {NaviItemView} This instance.
     */
    render: function() {
        this.$el.html(this.template(this.model.toJSON()));

        if (this.model.get('Current')) {
            this.$el.addClass('active');
        } else {
            this.$el.removeClass('current');
        }

        return this;
    }
});

/**
 * Manages the view for the navigation list.
 *
 * @constructor
 */
var NavView = Backbone.View.extend({
    /**
     * Initialization.
     *
     * @param {Object} options Initialization options.
     */
    initialize: function(options) {
        this.collection.bind('reset', this.render, this);
    },

    /**
     * Renders the view.
     *
     * @return {NavView} This instance.
     */
    render: function() {
        var model,
            view,
            i,
            n;

        this.$el.html('');

        for (i = 0, n = this.collection.length; i < n; i++) {
            model = this.collection.at(i);
            view = new NavItemView({model: model});
            this.$el.append(view.render().el);
        }

        return this;
    }
});