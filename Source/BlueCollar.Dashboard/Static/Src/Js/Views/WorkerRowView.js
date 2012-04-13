/**
 * List row view for Worker models.
 */
var WorkerRowView = Backbone.View.extend({
    events: {
        'click a.edit': 'edit',
        'click a.signal': 'signal'
    },
    tagName: 'tr',
    template: _.template($('#worker-row-template').html()),

    initialize: function(options) {
        this.model.bind('change', this.render, this);
    },

    edit: function() {
        this.trigger('edit', this);
    },

    render: function() {
        var el = $(this.el).html(this.template(this.model.toJSON()));

        if (this.model.get('editing')) {
            el.addClass('editing');
        } else {
            el.removeClass('editing');
        }

        return this;
    },

    signal: function() {
        this.trigger('signal', this);
    }
});