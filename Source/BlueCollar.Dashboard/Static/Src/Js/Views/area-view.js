/**
 * Serves as the base for area views.
 *
 * @constructor
 */
var AreaView = Backbone.View.extend({
    events: {
        'click button.btn-add': 'add'
    },

    /**
     * Initialization.
     *
     * @param {Object} options Initialization options.
     */
    initialize: function(options) {
        this.model.bind('change:Id', this.renderId, this);
        this.model.bind('change:Action', this.renderId, this);
        this.model.get('Collection').bind('reset', this.renderId, this);

        this.searchView = new SearchView({model: this.model});
        this.searchView.bind('submit', this.searchSubmit, this);
        this.searchView.bind('cancel', this.searchCancel, this);

        this.topPagerView = new PagerView({model: this.model});
        this.topPagerView.bind('page', this.page, this);

        this.bottomPagerView = new PagerView({model: this.model});
        this.bottomPagerView.bind('page', this.page, this);
    },

    /**
     * Handle's the add button's click event.
     */
    add: function() {},

    /**
     * Delegates delcarative events for the view.
     *
     * @param {Object} events A hash of additional events to delegate.
     */
    delegateEvents: function(events) {
        Backbone.View.prototype.delegateEvents.call(this, events);
        this.searchView.delegateEvents();
        this.topPagerView.delegateEvents();
        this.listView.delegateEvents();
        this.bottomPagerView.delegateEvents();
    },

    /**
     * Handles the list view's display event.
     *
     * @param {Object} sender The event sender.
     * @param {Object} args The event arguments.
     */
    display: function(sender, args) {
        this.model.set({Id: args.Model.get('Id'), Action: ''});
        this.trigger('display', this, args);
    },

    /**
     * Handles the display view's cancel event.
     *
     * @param {Object} sender The event sender.
     * @param {Object} args The event arguments.
     */
    displayCancel: function(sender, args) {
        this.model.clearId();
        sender.remove();
        this.trigger('editCancel', this, args);
    },

    /**
     * Handles the list view's edit event.
     *
     * @param {Object} sender The event sender.
     * @param {Object} args The event arguments.
     */
    edit: function(sender, args) {
        this.model.set({Id: args.Model.get('Id'), Action: ''});
        this.trigger('edit', this, args);
    },

    /**
     * Handles the edit view's cancel event.
     *
     * @param {Object} sender The event sender.
     * @param {Object} args The event arguments.
     */
    editCancel: function(sender, args) {
        this.model.clearId();
        sender.remove();
        this.trigger('editCancel', this, args);
    },

    /**
     * Handles the edit view's delete event.
     *
     * @param {Object} sender The event sender.
     * @param {Object} args The event arguments.
     */
    editDelete: function(sender, args) {
        this.model.clearId();
        sender.remove();
        this.trigger('editDelete', this, args);
    },

    /**
     * Handles the edit view's submit event.
     *
     * @param {Object} sender The event sender.
     * @param {Object} args The event arguments.
     */
    editSubmit: function(sender, args) {
        sender.showLoading();
        this.trigger('editSubmit', this, _.extend({}, args, {View: sender}));
    },

    /**
     * Handles a pager view's page event.
     *
     * @param {Object} sender The event sender.
     * @param {Object} args The event arguments.
     */
    page: function(sender, args) {
        this.model.set({PageNumber: args.PageNumber});  
        this.trigger('fetch', this, args);
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
            pagingFooterEl;

        this.searchView.$el.detach();
        this.topPagerView.$el.detach();
        this.listView.$el.detach();
        this.bottomPagerView.$el.detach();

        this.$el.html(this.template(this.model.toJSON()));

        searchEl = this.$('.search');
        pagingHeaderEl = this.$('.paging-header');
        listEl = this.$('.list');
        pagingFooterEl = this.$('.paging-footer');
        
        searchEl.html(this.searchView.render().el);
        pagingHeaderEl.html(this.topPagerView.render().el);
        listEl.html(this.listView.render().el);
        pagingFooterEl.html(this.bottomPagerView.render().el);

        this.renderId();

        return this;
    },

    /**
     * Checks whether the view for the selected ID should be rendered,
     * and calls renderIdView if necessary.
     */
    renderId: function() {
        var el = this.$('.details').html(''),
            model;
        
        if (this.model.get('Id')) {
            model = this.model.get('Collection').getSelected();

            if (model && model.get('Id') === this.model.get('Id')) {
                this.renderIdView(el, model);
            }
        }

        return this;
    },

    /**
     * Renders the ID view for the given model in the given details element.
     *
     * @param {jQuery} el The jQuery object containing the details element to render into.
     * @param {CollarModel} model The model to render the ID view for.
     */
    renderIdView: function(el, model) {},

    /**
     * Handles the list view's signal event.
     *
     * @param {Object} sender The event sender.
     * @param {Object} args The event arguments.
     */
    signal: function(sender, args) {
        this.model.set({Id: args.Model.get('Id'), Action: 'signal'});
        this.trigger('signal', this, args);
    },

    /**
     * Handles the search view's cancel event.
     *
     * @param {Object} sender The event sender.
     * @param {Object} args The event arguments.
     */
    searchCancel: function(sender, args) {
        this.model.set({Id: 0, PageNumber: 1, Search: ''});
        this.trigger('fetch', this, args);
    },

    /**
     * Handle's the search view's submit event.
     *
     * @param {Object} sender The event sender.
     * @param {Object} args The event arguments.
     */
    searchSubmit: function(sender, args) {
        this.model.set({Id: 0, PageNumber: 1, Search: args.Attributes.Search});
        this.trigger('fetch', this, args);
    },

    /**
     * Handles the signal view's submit event.
     *
     * @param {Object} sender The event sender.
     * @param {Object} args The event arguments.
     */
    signalSubmit: function(sender, args) {
        sender.showLoading();
        this.trigger('signalSubmit', this, _.extend({}, args, {View: sender}));
    }
});