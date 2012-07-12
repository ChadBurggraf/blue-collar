/**
 * Manages the row view for the scheduled jobs list.
 *
 * @constructor
 * @extends {RowView}
 */
var ScheduledJobsRowView = RowView.extend({
    template: _.template($('#scheduled-jobs-row-template').html())
});