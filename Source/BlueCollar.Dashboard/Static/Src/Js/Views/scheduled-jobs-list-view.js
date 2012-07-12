/**
 * Manages the scheduled jobs list view.
 *
 * @constructor
 * @extends {ListView}
 */
var ScheduledJobsListView = ListView.extend({
    cols: 2,
    template: _.template($('#scheduled-jobs-list-template').html()),

    /**
     * Renders the view's row collection.
     *
     * @param {jQuery} tbody The list's tbody element.
     * @param {CollarCollection} collection The collection to render rows for.
     * @return {ListView} This instance.
     */
    renderRows: function(tbody, collection) {
        var model,
            i,
            n;
        
        for (i = 0, n = collection.length; i < n; i++) {
            model = collection.at(i);
            view = new ScheduledJobsRowView({model: model}).render();
            view.bind('edit', this.edit, this);
            tbody.append(view.el);
        }

        return this;
    }
});