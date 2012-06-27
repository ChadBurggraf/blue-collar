/**
 * Manages the row view for the schedules list.
 *
 * @constructor
 * @extends {RowView}
 */
var SchedulesRowView = RowView.extend({
    template: _.template($('#schedules-row-template').html())
});