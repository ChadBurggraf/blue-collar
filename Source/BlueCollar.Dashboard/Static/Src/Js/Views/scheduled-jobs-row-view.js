/**
 * Manages the row view for the scheduled jobs list.
 *
 * @constructor
 * @extends {RowView}
 */
var ScheduledJobsRowView = RowView.extend({
    template: _.template($('#scheduled-jobs-row-template').html()),

    /**
     * Renders the view.
     *
     * @return {RowView} This instance.
     */
    render: function() {
        RowView.prototype.render.call(this);
        TimeoutQueue.enqueue('prettyPrint', prettyPrint);
        return this;
    }
});