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
        chartsLoaded: false,
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

    if (options.counts) {
        navCollection.reset(navCollection.parse(options.counts));
    } else {
        navCollection.fetch();
    }

    this.dashboardRouter = this.createRouter(DashboardRouter);
    this.historyRouter = this.createRouter(HistoryRouter);
    this.queueRouter = this.createRouter(QueueRouter);
    this.schedulesRouter = this.createRouter(SchedulesRouter);
    this.workersRouter = this.createRouter(WorkersRouter);
    this.workingRouter = this.createRouter(WorkingRouter);
      
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
    },

    /**
     * Sets a value indicating whether the charts API has been loaded.
     *
     * @param {boolean} loaded A value indicating whether the charts API has been loaded.
     */
    setChartsLoaded: function(loaded) {
        this.options.chartsLoaded = loaded;
        this.dashboardRouter.setChartsLoaded(loaded);
    }
});