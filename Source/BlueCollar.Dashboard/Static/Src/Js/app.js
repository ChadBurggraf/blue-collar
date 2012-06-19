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

    new DashboardRouter(this, this.options);
      
    new Backbone.History().start({
        pushState: true,
        root: this.urlRoot
    });
};