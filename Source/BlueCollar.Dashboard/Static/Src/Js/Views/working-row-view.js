/**
 * Manages the row view for the working job list.
 *
 * @constructor
 * @extends {RowView}
 */
var WorkingRowView = RowView.extend({
    template: _.template($('#working-row-template').html())
});