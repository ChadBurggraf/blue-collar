/**
 * Serves as the base for area views.
 *
 * @constructor
 */
var AreaView = Backbone.View.extend({
    /**
     * Initialization.
     *
     * @param {Object} options Initialization options.
     */
    initialize: function(options) {
        this.model.bind('change', this.render, this);

        this.searchView = new SearchView({model: this.model});
        this.searchView.bind('submit', this.submitSearch, this);
        this.searchView.bind('cancel', this.cancelSearch, this);

        this.topPagerView = new PagerView({model: this.model});
        this.topPagerView.bind('page', this.page, this);

        this.bottomPagerView = new PagerView({model: this.model});
        this.bottomPagerView.bind('page', this.page, this);
    },

    /**
     * Handles the search view's cancel event.
     *
     * @param {Object} sender The event sender.
     * @param {Object} args The event arguments.
     */
    cancelSearch: function(sender, args) {
        this.model.set({PageNumber: 1, Search: ''});
        this.trigger('fetch', this);
    },

    /**
     * Handles a pager view's page event.
     *
     * @param {Object} sender The event sender.
     * @param {Object} args The event arguments.
     */
    page: function(sender, args) {
        this.model.set({PageNumber: args.PageNumber});  
        this.trigger('fetch', this);
    },

    /**
     * Renders the view.
     *
     * @return {HistoryView} This instance.
     */
    render: function() {
        var searchEl,
            pagingHeaderEl,
            listEl,
            pagingFooterEl,
            detailsEl;

        this.searchView.$el.detach();
        this.topPagerView.$el.detach();
        this.listView.$el.detach();
        this.bottomPagerView.$el.detach();

        if (this.editView) {
            this.editView.$el.detach();
        }

        this.$el.html(this.template(this.model.toJSON()));

        searchEl = this.$('.search');
        pagingHeaderEl = this.$('.paging-header');
        listEl = this.$('.list');
        pagingFooterEl = this.$('.paging-footer');
        detailsEl = this.$('.details');

        searchEl.html(this.searchView.render().el);
        pagingHeaderEl.html(this.topPagerView.render().el);
        listEl.html(this.listView.render().el);
        pagingFooterEl.html(this.bottomPagerView.render().el);

        if (this.model.get('Editing') && this.editView) {
            detailsEl.html(this.editView.render().el);
        }

        return this;
    },

    /**
     * Handle's the search view's submit event.
     *
     * @param {Object} sender The event sender.
     * @param {Object} args The event arguments.
     */
    submitSearch: function(sender, args) {
        this.model.set({PageNumber: 1, Search: args.Search});
        this.trigger('fetch', this);
    }
});