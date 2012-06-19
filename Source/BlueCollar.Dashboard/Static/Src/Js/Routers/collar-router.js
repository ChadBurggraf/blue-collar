/**
 * Base router implementation.
 *
 * @constructor
 */
var CollarRouter = Backbone.Router.extend({
    /**
     * Initialization.
     * @this {CollarRouter}
     * @param {App} app The root application object.
     * @param {Object} options Additional initialization options.
     */
    initialize: function(app, options) {
        this.app = app;
    }
});