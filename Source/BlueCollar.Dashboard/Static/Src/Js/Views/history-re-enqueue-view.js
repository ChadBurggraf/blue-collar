/**
 * Implements the history re-enqueue form.
 *
 * @constructor
 * @extends {FormView}
 */
var HistoryReEnqueueView = FormView.extend({
    template: _.template($('#history-re-enqueue-template').html()),

    /**
     * Gets the name of the action performed by this instance upon submission.
     *
     * @return {String} The name of the action performed by this instance.
     */
    getAction: function() {
        return 're-enqueued';
    }
});