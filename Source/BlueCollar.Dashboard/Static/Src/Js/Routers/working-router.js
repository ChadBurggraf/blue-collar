/**
 * Working area router implementation.
 *
 * @constructor
 * @extends {CollarRouter}
 */
var WorkingRouter = CollarRouter.extend({
    name: 'Working',
    routes: {
        'working': 'index',
        'working/:search/p:page': 'index',
        'working//p:page': 'page',
        'working/*search': 'search'
    },

    /**
     * Initialization.
     *
     * @param {App} app The root application object.
     * @param {Object} options Initialization options.
     */
    initialize: function(app, options) {
        CollarRouter.prototype.initialize.call(this, app, options);
        this.controller = this.createController(WorkingController, 'working', this.options);
    }
});