/**
 * List row view for Queued models.
 */
var QueuedRowView = Backbone.View.extend({
    events: {
        'click a.display': 'display'
    },
    tagName: 'tr',
    template: _.template($('#queued-row-template').html()),

    initialize: function(options) {
        this.model.bind('change', this.render, this);
    },

    display: function() {
        this.trigger('display', this);
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