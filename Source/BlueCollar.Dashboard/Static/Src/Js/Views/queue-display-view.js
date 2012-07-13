/**
 * Implements the queue display form.
 *
 * @constructor
 * @extends {FormView}
 */
var QueueDisplayView = FormView.extend({
    template: _.template($('#queue-display-template').html()),

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
        TimeoutQueue.enqueue('prettyPrint', prettyPrint);
        return this;
    }
});