/**
 * List view for Schedule models.
 */
var ScheduleListView = ListView.extend({
    cols: 7,
    events: {
        'click button.add': 'add'
    },
    template: _.template($('#schedule-list-template').html()),

    manage: function(view) {
        this.trigger('manage', this, view.model);
    },

    renderRows: function(options) {
        var tbody = this.$("tbody").html(''),
            model,
            i,
            n;

        options = options || {};

        for (i = 0, n = this.collection.length; i < n; i++) {
            model = this.collection.at(i);
            view = new ScheduleRowView({model: model}).render();
            view.bind('edit', this.edit, this);
            view.bind('manage', this.manage, this);
            tbody.append(view.el);

            if (i === n - 1) {
                $(view.el).addClass("last");
            }
        }

        if (n === 0) {
            if (options.loading) {
                tbody.html(this.loading());
            } else {
                tbody.html(this.empty('There are no schedules to display.'));
            }
        }

        return this;
    }
});