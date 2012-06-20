/**
 * History area router implementation.
 *
 * @constructor
 * @extends {CollarRouter}
 */
var HistoryRouter = CollarRouter.extend({
    routes: {
        'history': 'index'
    },

    /**
     * Initialization.
     *
     * @param {App} app The root application object.
     * @param {Object} options Additional initialization options.
     */
    initialize: function(app, options) {
        CollarRouter.prototype.initialize.call(this, app, options);
        this.options = _.extend({}, options);
    },

    /**
     * Handles the root #history route.
     */
    index: function() {
        
    }
});