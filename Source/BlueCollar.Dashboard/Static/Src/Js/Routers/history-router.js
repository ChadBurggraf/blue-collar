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
        'history/id/:id': 'id',
        'history/q/:search': 'search',
        'history/q/:search/id/:id': 'searchId',
        'history/p/:page': 'page',
        'history/p/:page/id/:id': 'pageId',
        'history/q/:search/p/:page': 'searchPage',
        'history/q/:search/p/:page/id/:id': 'index'
    },

    /**
     * Initialization.
     *
     * @param {App} app The root application object.
     * @param {Object} options Initialization options.
     */
    initialize: function(app, options) {
        CollarRouter.prototype.initialize.call(this, app, options);
        this.controller = this.createController(HistoryController, this.options);
    },
});