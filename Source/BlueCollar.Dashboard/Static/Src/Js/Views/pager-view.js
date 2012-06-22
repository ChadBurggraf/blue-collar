var PagerView = Backbone.View.extend({
    className: 'pagination',
    events: {
        'form submit': 'submit',
        '.pagination-previous a': 'previous',
        'a.pagination-count': 'last',
        '.pagination-next a': 'next'
    },
    tagName: 'div',
    template: _.template($('#pager-template').html()),

    initialize: function(options) {
        this.model.bind('change', this.render, this);
    },

    last: function() {

    },

    next: function() {

    },

    previous: function() {

    },

    render: function() {
        var number = this.model.get('PageNumber'),
            count = this.model.get('PageCount');

        if (count > 1) {
            this.$el.show().html(this.template(this.model.toJSON()));

            if (number === 1) {
                this.$('.pagination-previous').addClass('disabled');
            }

            if (number === count) {
                this.$('a.pagination-count').hide();
                this.$('.pagination-next').addClass('disabled');
            } else {
                this.$('span.pagination-count').hide();
            }
        } else {
            this.$el.hide();
        }

        return this;
    },

    submit: function() {

    }
});