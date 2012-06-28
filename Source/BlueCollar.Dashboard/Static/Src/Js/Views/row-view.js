/**
 * Provides a base for list row view implementations.
 *
 * @constructor
 */
var RowView = Backbone.View.extend({
    events: {
        'click .btn-display': 'display',
        'click .btn-edit': 'edit',
        'click .btn-signal': 'signal'
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
        this.trigger('display', this, {Model: this.model});
    },

    /**
     * Handles the row's edit event.
     */
    edit: function() {
        this.trigger('edit', this, {Model: this.model});
    },

    /**
     * Renders the view.
     *
     * @return {RowView} This instance.
     */
    render: function() {
        this.$el.html(this.template(this.model.toJSON()));

        if (this.model.get('Selected')) {
            this.$el.addClass('selected');
        } else {
            this.$el.removeClass('selected');   
        }

        return this;
    },

    /**
     * Handles the row's signal event.
     */
    signal: function() {
        this.trigger('signal', this, {Model: this.model});
    }
});