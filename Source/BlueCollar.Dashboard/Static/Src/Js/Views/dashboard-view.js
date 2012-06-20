/**
 * Manages the root dashboard view.
 *
 * @constructor
 */
var DashboardView = Backbone.View.extend({
    template: _.template($('#dashboard-template').html()),

    /**
     * Renders the view.
     *
     * @return {DashboardView} This instance.
     */
    render: function() {
        this.$el.html(this.template());
        return this;
    }
});