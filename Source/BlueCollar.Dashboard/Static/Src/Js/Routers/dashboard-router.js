/**
 * Dashboard area router implementation.
 *
 * @constructor
 * @extends {CollarRouter}
 */
var DashboardRouter = CollarRouter.extend({
    name: 'Dashboard',
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
     * Sets a value indicating whether the charts API has been loaded.
     *
     * @param {boolean} loaded A value indicating whether the charts API has been loaded.
     */
    setChartsLoaded: function(loaded) {
        this.options.chartsLoaded = loaded;
        this.controller.setChartsLoaded(loaded);
    }
});