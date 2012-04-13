/**
 * List row view for History models.
 */
var HistoryRowView = Backbone.View.extend({
    events: {
        'click a.display': 'display'
    },
    tagName: 'tr',
    template: _.template($('#history-row-template').html()),

    initialize: function(options) {
        this.model.bind('change', this.render, this);
    },

    display: function() {
        this.trigger('display', this);
    },

    render: function() {
        var el = $(this.el).html(this.template(this.model.toJSON())),
            status = this.model.get('Status'),
            css = '';

        switch (status) {
            case 'Succeeded':
                css = 'green';
                break;
            case 'Failed':
            case 'TimedOut':
                css = 'red';
                break;
            default:
                break;
        }

        this.$('.status').removeClass('red green').addClass(css);

        if (this.model.get('editing')) {
            el.addClass('editing');
        } else {
            el.removeClass('editing');
        }

        return this;
    }
});