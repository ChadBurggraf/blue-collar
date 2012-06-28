/**
 * Manages a paging view for paging through a list.
 *
 * @constructor
 */
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

    /**
     * Initialization.
     *
     * @param {Object} options Initialization options.
     */
    initialize: function(options) {
        this.model.bind('change:PageNumber', this.render, this);
        this.model.bind('change:PageCount', this.render, this);
    },

    /**
     * Handles the keyup event.
     *
     * @param {DOMEvent} event The event definition.
     */
    keyup: function(event) {
        if (event.keyCode === 13) {
            this.submit();
        }   
    },

    /**
     * Handles the last event.
     */
    last: function() {
        this.page(this.model.get('PageCount'));
    },

    /**
     * Handles the next event.
     */
    next: function() {
        this.page(this.model.get('PageNumber') + 1);
    },

    /**
     * Raises the 'page' event with the given page number.
     *
     * @param {Number} pageNumber The page number to raise the event with.
     */
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

    /**
     * Handles the previous event.
     */
    previous: function() {
        this.page(this.model.get('PageNumber') - 1);
    },

    /**
     * Renders the view.
     *
     * @return {PagerView} This instance.
     */
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

    /**
     * Handles the submit event.
     */
    submit: function() {
        var pageNumber = parseInt(this.$('input').val(), 10);

        if (!isNaN(pageNumber) && pageNumber > 0) {
            this.page(pageNumber);
        }
    }
});