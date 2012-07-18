/**
 * Implements the history re-enqueue form.
 *
 * @constructor
 * @extends {FormView}
 */
var HistoryReEnqueueView = FormView.extend({
    template: _.template($('#history-re-enqueue-template').html()),

    /**
     * Focuses the first element in the form.
     */
    focus: function() {
        this.$('textarea').focus();
    },

    /**
     * Gets the name of the action performed by this instance upon submission.
     *
     * @return {String} The name of the action performed by this instance.
     */
    getAction: function() {
        return 're-enqueued';
    }
});