/**
 * Implements the history re-enqueue form.
 *
 * @constructor
 * @extends {FormView}
 */
var HistoryReEnqueueView = FormView.extend({
    template: _.template($('#history-re-enqueue-template').html())
});