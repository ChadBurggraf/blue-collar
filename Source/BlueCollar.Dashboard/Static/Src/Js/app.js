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

    new DashboardRouter(this, this.options);
    new HistoryRouter(this, this.options);
    new QueueRouter(this, this.options);
    new SchedulesRouter(this, this.options);
    new WorkersRouter(this, this.options);
    new WorkingRouter(this, this.options);
      
    Backbone.history.start({
        pushState: true,
        root: this.urlRoot
    });
};