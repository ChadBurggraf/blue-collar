/**
 * Manages the root history view.
 *
 * @constructor
 */
var HistoryView = Backbone.View.extend({
    template: _.template($('#history-template').html()),

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
     * @return {HistoryView} This instance.
     */
    render: function() {
        this.$el.html(this.template(this.model.toJSON()));
        return this;
    }
});