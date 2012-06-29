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
        'workers/id/:id': 'id',
        'workers/id/:id/:action': 'idAction',
        'workers/q/:search': 'search',
        'workers/q/:search/id/:id': 'searchId',
        'workers/q/:search/id/:id/:action': 'searchIdAction',
        'workers/p/:page': 'page',
        'workers/p/:page/id/:id': 'pageId',
        'workers/p/:page/id/:id/:action': 'pageIdAction',
        'workers/q/:search/p/:page': 'searchPage',
        'workers/q/:search/p/:page/id/:id/:action': 'index'
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