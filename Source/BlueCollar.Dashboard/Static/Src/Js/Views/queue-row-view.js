/**
 * Manages the row view for the queue list.
 *
 * @constructor
 * @extends {RowView}
 */
var QueueRowView = RowView.extend({
    template: _.template($('#queue-row-template').html())
});