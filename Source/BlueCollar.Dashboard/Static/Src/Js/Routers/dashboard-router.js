/**
 * Dashboard area router implementation.
 *
 * @constructor
 * @extends {CollarRouter}
 */
var DashboardRouter = CollarRouter.extend({
    routes: {
        'dashboard': 'index',
        '*path': 'index'
    },

    /**
     * Initialization.
     *
     * @param {App} app The root application object.
     * @param {Object} options Initialization options.
     */
    initialize: function(app, options) {
        CollarRouter.prototype.initialize.call(this, app, options);
        this.controller = this.createController(DashboardController, 'stats', this.options);
    },

    /**
     * Handles the root #dashboard route.
     */
    index: function() {
        this.controller.index();
        this.trigger('nav', this, {name: 'Dashboard'});
    }
});