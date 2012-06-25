/**
 * Provides a base for list row view implementations.
 *
 * @constructor
 */
var RowView = Backbone.View.extend({
    events: {
        'a.display': 'display'
    },
    tagName: 'tr',

    /**
     * Initialization.
     *
     * @param {Object} options Initialization options.
     */
    initialize: function(options) {
        this.model.bind('change', this.render, this);
    },

    /**
     * Handles the row's display event.
     */
    display: function() {
        this.trigger('display', this);
    },

    /**
     * Renders the view.
     *
     * @return {RowView} This instance.
     */
    render: function() {
        this.$el.html(this.template(this.model.toJSON()));

        if (this.model.get('Editing')) {
            this.$el.addClass('editing');
        } else {
            this.$el.removeClass('editing');   
        }

        return this;
    }
});