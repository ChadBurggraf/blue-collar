/**
 * Base router implementation.
 *
 * @constructor
 */
var CollarRouter = Backbone.Router.extend({
    controller: CollarController,

    /**
     * Initialization.
     *
     * @param {App} app The root application object.
     * @param {Object} options Initialization options.
     */
    initialize: function(app, options) {
        this.app = app;
    }
});