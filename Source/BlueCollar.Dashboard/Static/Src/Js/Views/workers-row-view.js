/**
 * Manages the row view for the workers list.
 *
 * @constructor
 * @extends {RowView}
 */
var WorkersRowView = RowView.extend({
    template: _.template($('#workers-row-template').html())
});