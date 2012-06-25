/**
 * Manages the root history view.
 *
 * @constructor
 */
var HistoryView = Backbone.View.extend({
    template: _.template($('#history-template').html()),

    /**
     * Initialization.
     *
     * @param {Object} options Initialization options.
     */
    initialize: function(options) {
        this.model.get('Collection').bind('reset', this.reset, this);

        this.searchView = new SearchView({model: this.model});
        this.searchView.bind('submit', this.submitSearch, this);
        this.searchView.bind('cancel', this.cancelSearch, this);

        this.topPagerView = new PagerView({model: this.model});
        this.topPagerView.bind('page', this.page, this);

        this.listView = new HistoryListView({model: this.model});
        this.listView.bind('display', this.display, this);

        this.bottomPagerView = new PagerView({model: this.model});
        this.bottomPagerView.bind('page', this.page, this);
    },

    cancelSearch: function(sender, args) {
        //this.model.set({Search: ''});
        //this.triggerFetch();
    },

    display: function(sender, args) {

    },

    page: function(sender, args) {
        //this.model.set({PageNumber: args.PageNumber});  
        //this.triggerFetch();
    },

    /**
     * Renders the view.
     *
     * @return {HistoryView} This instance.
     */
    render: function(options) {
        var searchEl,
            pagingHeaderEl,
            listEl,
            pagingFooterEl,
            detailsEl;

        options = options || {};
        this.$el.html(this.template(this.model.toJSON()));

        searchEl = this.$('.search');
        pagingHeaderEl = this.$('.paging-header');
        listEl = this.$('.list');
        pagingFooterEl = this.$('.paging-footer');
        detailsEl = this.$('.details');

        searchEl.html(this.searchView.render(options).el);
        pagingHeaderEl.html(this.topPagerView.render(options).el);
        listEl.html(this.listView.render(options).el);
        pagingFooterEl.html(this.bottomPagerView.render(options).el);

        return this;
    },

    reset: function() {
        //var collection = this.model.get('Collection');
        //this.model.set({PageNumber: collection.pageNumber, PageCount: collection.pageCount});
    },

    submitSearch: function(sender, args) {
        //this.model.set({Search: args.Search});
        //this.triggerFetch();
    },

    triggerFetch: function() {
        //this.model.set({Loading: true});
        //this.trigger('fetch', this);
    }
});