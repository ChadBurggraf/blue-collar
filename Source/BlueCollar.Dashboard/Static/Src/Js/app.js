/**
 * Application entry point.
 *
 * @constructor
 * @param {String} name The name of the application.
 * @param {String} urlRoot The root application URL, used for navigation and Ajax.
 * @param {Object} options Additional initialization options.
 */
 var App = function(name, urlRoot, options) {
    var navCollection;

    this.options = options = _.extend({
        stats: null,
        showCounts: true,
        testLink: false
    }, options);

    this.name = name || 'Default';
    this.urlRoot = (urlRoot || '/').withTrailingSlash();
    this.jsonUrlRoot = this.options.jsonUrlRoot ? this.options.jsonUrlRoot.withTrailingSlash() : this.urlRoot;

    this.nav = $('#nav');
    this.page = $('#page');

    navCollection = new NavCollection();
    navCollection.urlRoot = this.urlRoot;
    navCollection.showCounts = this.options.showCounts;
    navCollection.testLink = this.options.testLink;
    navCollection.url = this.jsonUrlRoot + 'counts';
    this.navView = new NavView({collection: navCollection, el: this.nav});

    if (options.stats && options.stats.Counts) {
        navCollection.reset(navCollection.parse(options.stats.Counts));
    } else {
        navCollection.fetch();
    }

    this.createRouter(DashboardRouter);
    this.createRouter(HistoryRouter);
    this.createRouter(QueueRouter);
    this.createRouter(SchedulesRouter);
    this.createRouter(WorkersRouter);
    this.createRouter(WorkingRouter);
      
    Backbone.history.start();
};

/**
 * Prototype functions.
 */
_.extend(App.prototype, {
    /**
     * Handles counts updated events.
     *
     * @param {Object} sender The event sender.
     * @param {Object} args The event arguments.
     */
    counts: function(sender, args) {
        if (args && args.Counts) {
            this.navView.collection.reset(this.navView.collection.parse(args.Counts));
        }
    },

    /**
     * Creates a new {CollarRouter} using the given constructor function.
     *
     * @param {Function} router The constructor function to use.
     * @return {CollarRouter} The constructed router.
     */
    createRouter: function(router) {
        var r = new router(this, this.options);
        r.bind('counts', this.counts, this);
        r.bind('nav', this.navigated, this);
        return r;
    },

    /**
     * Handles navigated events.
     *
     * @param {Object} sender The event sender.
     * @param {Object} args The event arguments.
     */
    navigated: function(sender, args) {
        if (args && args.name) {
            this.navView.collection.setCurrent(args.name);
        }
    }
});