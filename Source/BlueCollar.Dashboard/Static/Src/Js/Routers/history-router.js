/**
 * History area router implementation.
 *
 * @constructor
 * @extends {CollarRouter}
 */
var HistoryRouter = CollarRouter.extend({
    name: 'History',
    routes: {
        'history': 'index',
        'history/:search/p:page': 'index',
        'history//p:page': 'page',
        'history/*search': 'search'
    },

    /**
     * Initialization.
     *
     * @param {App} app The root application object.
     * @param {Object} options Initialization options.
     */
    initialize: function(app, options) {
        CollarRouter.prototype.initialize.call(this, app, options);
        this.controller = this.createController(HistoryController, 'history', this.options);
    }
});