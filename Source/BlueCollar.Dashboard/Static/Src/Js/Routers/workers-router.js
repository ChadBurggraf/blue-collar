/**
 * Workers area router implementation.
 *
 * @constructor
 * @extends {CollarRouter}
 */
var WorkersRouter = CollarRouter.extend({
    name: 'Workers',
    routes: {
        'workers': 'index',
        'workers/:search/p:page': 'index',
        'workers//p:page': 'page',
        'workers/*search': 'search'
    },

    /**
     * Initialization.
     *
     * @param {App} app The root application object.
     * @param {Object} options Initialization options.
     */
    initialize: function(app, options) {
        CollarRouter.prototype.initialize.call(this, app, options);
        this.controller = this.createController(WorkersController, 'workers', this.options);
    }
});