/**
 * Item view for NavigationItem models.
 */
var NavigationItemView = Backbone.View.extend({
    tagName: 'li',
    template: _.template($('#navigation-item-template').html()),

    initialize: function(options) {
        this.model.bind('change', this.render, this);
    },

    render: function() {
        var el = $(this.el);
        el.html(this.template(this.model.toJSON()));

        if (this.model.get('Current')) {
            this.$('a').addClass('current');
        } else {
            this.$('a').removeClass('current');
        }

        return this;
    }
});

/**
 * List view for NavigationItem collections.
 * Renders the main left-hand navigation.
 */
var NavigationView = Backbone.View.extend({
    el: $('#nav'),

    initialize: function(options) {
        this.collection.bind('reset', this.render, this);
    },

    render: function() {
        var el = $(this.el).html(''),
            model,
            view,
            i,
            n;

        for (i = 0, n = this.collection.length; i < n; i++) {
            model = this.collection.at(i);
            view = new NavigationItemView({model: model});
            el.append(view.render().el);
        }

        return this;
    }
});