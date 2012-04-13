/**
 * Pager view.
 */
var PagerView = Backbone.View.extend({
    events: {
        'submit': 'submit'
    },
    tagName: 'form',

    initialize: function(options) {
        $(this.el).attr('action', 'javascript:void(0);');
        this.model.bind('change', this.render, this);
    },

    keyup: function(event) {
        if (event.keyCode === 13) {
            this.submit();
        }
    },

    page: function(pageNumber) {
        if (_.isNumber(pageNumber) 
            && pageNumber > 0
            && pageNumber !== this.model.get('PageNumber')
            && pageNumber <= this.model.get('PageCount')) {
            this.trigger('page', this, pageNumber);
        }
    },

    render: function() {
        var pageNumber = this.model.get('PageNumber'),
            pageCount = this.model.get('PageCount'),
            el = $(this.el).html(''),
            ul = $('<ul/>').appendTo(el),
            liPrev = $('<li/>').appendTo(ul),
            liPage = $('<li/>').appendTo(ul),
            liNext = $('<li/>').appendTo(ul),
            label = $('<label/>').appendTo(liPage),
            input = $('<input type="text" name="PageNumber" />')
                .bind('keyup', _.bind(this.keyup, this))
                .bind('change', _.bind(this.submit, this))
                .val(pageNumber);

        if (pageNumber > 1) {
            $('<a href="javascript:void(0);"/>')
                .text('« Prev')
                .bind('click', _.bind(this.page, this, pageNumber - 1))
                .appendTo(liPrev);
        } else {
            liPrev.addClass('disabled');
            $('<span/>').text('« Prev').appendTo(liPrev);
        }

        label
            .append($('<span/>').text('Page'))
            .append(input)
            .append($('<span/>').text('of'));

        if (pageNumber !== pageCount) {
            $('<a href="javascript:void(0);"/>')
                .text(new Number(pageCount).format('0,000'))
                .bind('click', _.bind(this.page, this, pageCount))
                .appendTo(liPage);

            $('<a href="javascript:void(0);"/>')
                .addClass('last')
                .text('Next »')
                .bind('click', _.bind(this.page, this, pageNumber + 1))
                .appendTo(liNext);
        } else {
            $('<span/>').text(new Number(pageCount).format('0,000')).appendTo(liPage);
            $('<span/>').addClass('last').text('Next »').appendTo(liNext);
        }

        return this;
    },

    submit: function() {
        var pageNumber = parseInt(this.$('input').val());

        if (!isNaN(pageNumber) && pageNumber > 0) {
            this.page(pageNumber);
        }
    }
});