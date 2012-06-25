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
        this.model.bind('change', this.change, this);

        this.searchView = new SearchView({model: new Backbone.Model({Search: this.model.get('Search')})});
        this.searchView.bind('submit', this.submitSearch, this);
        this.searchView.bind('cancel', this.cancelSearch, this);

        this.topPagerView = new PagerView({model: new Backbone.Model(this.getPagingAttributes())});
        this.topPagerView.bind('page', this.page, this);

        this.bottomPagerView = new PagerView({model: new Backbone.Model(this.getPagingAttributes())});
        this.bottomPagerView.bind('page', this.page, this);
    },

    change: function(sender, args) {
        var pagingAttributes = this.getPagingAttributes();

        this.searchView.model.set({Search: this.model.get('Search')});
        this.topPagerView.set(pagingAttributes);
        this.bottomPagerView.set(pagingAttributes);
    },

    cancelSearch: function(sender, args) {
        this.model.set({Search: ''});
    },

    getPagingAttributes: function() {
        var attr = {PageNumber: this.model.get('PageNumber'), PageCount: this.model.get('PageCount')};

        if (_.isUndefined(attr.PageCount) || attr.PageCount < 1) {
            attr.PageCount = 1;
        }

        return attr;
    },

    page: function(sender, args) {
        this.model.set({PageNumber: args.PageNumber});  
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

        this.$el.html(this.template(this.model.toJSON()));

        searchEl = this.$('.search');
        pagingHeaderEl = this.$('.paging-header');
        listEl = this.$('.list');
        pagingFooterEl = this.$('.paging-footer');
        detailsEl = this.$('.details');

        this.searchView.model.set({Search: this.model.get('Search')}, {silent: true});
        searchEl.html(this.searchView.render().el);

        this.topPagerView.model.set(this.getPagingAttributes(), {silent: true});
        pagingHeaderEl.html(this.topPagerView.render().el);

        this.bottomPagerView.model.set(this.getPagingAttributes(), {silent: true});
        pagingFooterEl.html(this.bottomPagerView.render().el);

        return this;
    },

    submitSearch: function(sender, args) {
        this.model.set({Search: args.Search});
    }
});