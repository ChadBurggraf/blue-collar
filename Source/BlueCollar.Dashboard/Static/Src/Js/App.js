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
    this.urlRoot = urlRoot || '/';

    if (this.urlRoot.lastIndexOf('/') !== this.urlRoot.length - 1) {
        this.urlRoot += '/';
    }

    if (this.options.jsonUrlRoot) {
        this.jsonUrlRoot = this.options.jsonUrlRoot;

        if (this.jsonUrlRoot.lastIndexOf('/') !== this.jsonUrlRoot.length - 1) {
            this.jsonUrlRoot += '/';
        }
    } else {
        this.jsonUrlRoot = this.urlRoot;
    }

    this.dashboard = new Dashboard(this, this.options);
    Backbone.history.start();
};