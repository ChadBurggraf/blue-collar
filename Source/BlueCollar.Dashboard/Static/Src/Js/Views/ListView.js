/**
 * Base view implementation for list views. Provides
 * rendering and search/paging behaviors.
 */
var ListView = Backbone.View.extend({
    cols: 1,
    tagName: 'div',

    initialize: function(options) {
        this.collection.bind('counts', this.counts, this);
        this.collection.bind('reset', this.render, this);
    },

    add: function() {
        this.trigger('add', this);
    },

    counts: function(collection, counts) {
        this.trigger('counts', this, counts);
    },

    edit: function(view) {
        this.trigger('edit', this, view.model);
    },

    empty: function(message) {
        return $('<tr class="empty"/>').append(
            $('<td/>').attr('colspan', this.cols).append(
                $('<p/>').text(message)));
    },

    loading: function() {
        return $('<tr class="loading"/>').append(
            $('<td/>').attr('colspan', this.cols).append(
                $('<p/>').append(
                    $('<span class="loading"/>'))));
    },

    page: function(pagerView, pageNumber) {
        this.trigger('page', this, pageNumber);
    },

    render: function(options) {
        $(this.el).html(this.template());
        this.renderHeader(options);
        this.renderRows(options);
        this.renderFooter(options);
        return this;
    },

    renderFooter: function(options) {
        this.renderPager(this.$('.list-footer .pagination'), options);
        return this;
    },

    renderHeader: function(options) {
        var searchView = new SearchFormView({model: new Backbone.Model()});
        
        searchView.model.set({Search: this.collection.search});
        this.$(".search").html(searchView.render().el);
        searchView.bind('submit', this.searchSubmit, this);
        searchView.bind('clear', this.searchClear, this);

        this.renderPager(this.$('.list-header .pagination'), options);
        
        return this;
    },

    renderPager: function(container, options) {
        var pager = new Pager(),
            pagerView;

        pager.set(pager.parse(this.collection));

        if (pager.get('PageCount') > 1) {
            pagerView = new PagerView({model: pager});
            pagerView.bind('page', this.page, this);
            container.html(pagerView.render().el).show();
        } else {
            container.html('').hide();
        }

        return this;
    },

    renderRows: function(options) {
        return this;
    },

    searchClear: function() {
        this.trigger('search', this, '');
    },

    searchSubmit: function(view, attributes) {
        this.trigger('search', this, attributes);
    }
});