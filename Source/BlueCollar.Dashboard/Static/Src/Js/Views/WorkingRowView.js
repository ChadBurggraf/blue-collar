/**
 * List row view for Working models.
 */
var WorkingRowView = Backbone.View.extend({
    events: {
        'click a.display': 'display',
        'click a.signal': 'signal'
    },
    tagName: 'tr',
    template: _.template($('#working-row-template').html()),

    initialize: function(options) {
        this.model.bind('change', this.render, this);
    },

    display: function() {
        this.trigger('display', this);
    },

    signal: function() {
        this.trigger('signal', this);
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