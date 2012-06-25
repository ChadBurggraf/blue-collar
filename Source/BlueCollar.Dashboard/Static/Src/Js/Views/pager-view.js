var PagerView = Backbone.View.extend({
    className: 'pagination',
    events: {
        'submit form': 'submit',
        'click .pagination-previous a': 'previous',
        'keyup input': 'keyup',
        'change input': 'submit',
        'click a.pagination-count': 'last',
        'click .pagination-next a': 'next'
    },
    tagName: 'div',
    template: _.template($('#pager-template').html()),

    initialize: function(options) {
        this.model.bind('change', this.render, this);
    },

    keyup: function(event) {
        if (event.keyCode === 13) {
            this.submit();
        }   
    },

    last: function() {
        this.page(this.model.get('PageCount'));
    },

    next: function() {
        this.page(this.model.get('PageNumber') + 1);
    },

    page: function(pageNumber) {
        var cn = this.model.get('PageNumber'),
            cc = this.model.get('PageCount');

        if (pageNumber < 1) {
            pageNumber = 1;
        }

        if (pageNumber !== 1 && _.isNumber(cc)) {
            if (pageNumber > cc) {
                pageNumber = cc;
            }
        } else {
            pageNumber = 1;
        }

        if (pageNumber !== cn) {
            this.trigger('page', this, {PageNumber: pageNumber});
        }
    },

    previous: function() {
        this.page(this.model.get('PageNumber') - 1);
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
        var pageNumber = parseInt(this.$('input').val(), 10);

        if (!isNaN(pageNumber) && pageNumber > 0) {
            this.page(pageNumber);
        }
    }
});