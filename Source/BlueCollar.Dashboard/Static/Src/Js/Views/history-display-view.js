/**
 * Implements the history display form.
 *
 * @constructor
 * @extends {FormView}
 */
var HistoryDisplayView = FormView.extend({
    template: _.template($('#history-display-template').html()),

    /**
     * Handles model change events.
     */
    change: function() {
        this.render();
    },

    /**
     * Renders the view.
     *
     * @return {FormView} This instance.
     */
    render: function() {
        FormView.prototype.render.call(this);

        if (!this.model.get('Exception')) {
            this.$('.exception').remove();
        }

        setTimeout(prettyPrint, 100);
        return this;
    }
});