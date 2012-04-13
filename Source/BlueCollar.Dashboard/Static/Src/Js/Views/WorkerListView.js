/**
 * List view for Worker models.
 */
var WorkerListView = ListView.extend({
    cols: 6,
    events: {
        'click button.add': 'add'
    },
    template: _.template($('#worker-list-template').html()),

    renderRows: function(options) {
        var tbody = this.$("tbody").html(''),
            model,
            i,
            n;

        options = options || {};

        for (i = 0, n = this.collection.length; i < n; i++) {
            model = this.collection.at(i);
            view = new WorkerRowView({model: model}).render();
            view.bind('edit', this.edit, this);
            view.bind('signal', this.signal, this);
            tbody.append(view.el);

            if (i === n - 1) {
                $(view.el).addClass("last");
            }
        }

        if (n === 0) {
            if (options.loading) {
                tbody.html(this.loading());
            } else {
                tbody.html(this.empty('There are no workers to display.'));
            }
        }

        return this;
    },

    signal: function(view) {
        this.trigger('signal', this, view.model);
    }
});