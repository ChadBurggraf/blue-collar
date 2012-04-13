/**
 * List view for ScheduledJob models.
 */
var ScheduledJobListView = ListView.extend({
    cols: 2,
    events: {
        'click button.add': 'add'
    },
    template: _.template($('#scheduled-job-list-template').html()),

    renderHeader: function(options) {
        ListView.prototype.renderHeader.call(this, options);
        this.$('a.schedule').text(this.collection.ScheduleName);
        return this;
    },

    renderRows: function(options) {
        var tbody = this.$("tbody").html(''),
            model,
            i,
            n;

        options = options || {};

        for (i = 0, n = this.collection.length; i < n; i++) {
            model = this.collection.at(i);
            view = new ScheduledJobRowView({model: model}).render();
            view.bind('edit', this.edit, this);
            tbody.append(view.el);

            if (i === n - 1) {
                $(view.el).addClass("last");
            }
        }

        if (n === 0) {
            if (options.loading) {
                tbody.html(this.loading());
            } else {
                tbody.html(this.empty('There are no jobs to display.'));
            }
        }

        return this;
    }
});