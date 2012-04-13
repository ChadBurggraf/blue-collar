/**
 * List row view for Schedule models.
 */
var ScheduleRowView = Backbone.View.extend({
    events: {
        'click a.edit': 'edit',
        'click a.manage': 'manage'
    },
    tagName: 'tr',
    template: _.template($('#schedule-row-template').html()),

    initialize: function(options) {
        this.model.bind('change', this.render, this);
    },

    edit: function() {
        this.trigger('edit', this);
    },

    manage: function() {
        this.trigger('manage', this);
    },

    render: function() {
        var el = $(this.el).html(this.template(this.model.toJSON()));

        if (this.model.get('editing')) {
            el.addClass('editing');
        } else {
            el.removeClass('editing');
        }

        return this;
    }
});