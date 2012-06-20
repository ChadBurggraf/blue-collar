/**
 * Application entry point.
 *
 * @constructor
 * @this {App}
 * @param {string} urlRoot The root application URL, used for navigation and Ajax.
 * @param {object} options Additional initialization options.
 */
 var App = function(urlRoot, options) {
    this.options = options || {};
    this.urlRoot = (urlRoot || '/').withTrailingSlash();
    this.jsonUrlRoot = this.options.jsonUrlRoot ? this.options.jsonUrlRoot.withTrailingSlash() : this.urlRoot;

    this.nav = $('#nav');
    this.page = $('#page');

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