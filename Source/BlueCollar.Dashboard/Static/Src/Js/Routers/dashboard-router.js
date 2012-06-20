/**
 * Dashboard area router implementation.
 *
 * @constructor
 * @extends {CollarRouter}
 */
var DashboardRouter = CollarRouter.extend({
    controller: DashboardController,
    routes: {
        'dashboard': 'index',
        '*path': 'index'
    },

    /**
     * Initialization.
     * @this {DashboardRouter}
     * @param {App} app The root application object.
     * @param {Object} options Initialization options.
     */
    initialize: function(app, options) {
        CollarRouter.prototype.initialize.call(this, app, options);
        this.options = _.extend({}, options);
    },

    /**
     * Handles the root #dashboard route.
     * @this {DashboardRouter}
     */
    index: function() {
        new this.controller(this.app.nav, this.app.page);
    }
});