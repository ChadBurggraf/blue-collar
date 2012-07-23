/**
 * Manages the scheduled jobs list view.
 *
 * @constructor
 * @extends {ListView}
 */
var ScheduledJobsListView = ListView.extend({
    cols: 3,
    template: _.template($('#scheduled-jobs-list-template').html()),

    /**
     * Handles a row's number event.
     *
     * @param {Object} sender The event sender.
     * @param {Object} args The event arguments.
     */
    number: function(sender, args) {
        this.trigger('number', this, _.extend(args, {View: sender}));
    },

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
            view.bind('number', this.number, this);
            tbody.append(view.el);
        }

        return this;
    }
});