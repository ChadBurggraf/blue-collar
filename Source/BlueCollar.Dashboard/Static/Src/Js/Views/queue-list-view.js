/**
 * Manages the queue list view.
 *
 * @constructor
 * @extends {ListView}
 */
var QueueListView = ListView.extend({
    cols: 5,
    template: _.template($('#queue-list-template').html()),

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
            view = new QueueRowView({model: model}).render();
            view.bind('display', this.display, this);
            tbody.append(view.el);
        }

        return this;
    }
});