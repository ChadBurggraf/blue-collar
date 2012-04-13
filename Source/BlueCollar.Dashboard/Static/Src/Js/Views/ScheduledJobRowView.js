/**
 * List row view for ScheduledJob models.
 */
var ScheduledJobRowView = Backbone.View.extend({
    events: {
        'click a.edit': 'edit'
    },
    tagName: 'tr',
    template: _.template($('#scheduled-job-row-template').html()),

    initialize: function(options) {
        this.model.bind('change', this.render, this);
    },

    edit: function() {
        this.trigger('edit', this);
    },

    render: function() {
        var el = $(this.el).html(this.template(this.model.toJSON())),
            propertiesUl = this.$('ul.properties').html(''),
            properties = JSON.parse(this.model.get('Properties') || '{}'),
            prop;

        for (prop in properties) {
            if (properties.hasOwnProperty(prop)) {
                propertiesUl.append($('<li/>').text(prop + ' ').append($('<em/>').text(properties[prop])));
            }
        }

        if (this.model.get('editing')) {
            el.addClass('editing');
        } else {
            el.removeClass('editing');
        }

        return this;
    }
});