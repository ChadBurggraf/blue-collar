/**
 * Queue area router implementation.
 *
 * @constructor
 * @extends {CollarRouter}
 */
var QueueRouter = CollarRouter.extend({
    name: 'Queue',
    routes: {
        'queue': 'index',
        'queue/id/:id': 'id',
        'queue/id/:id/:action': 'idAction',
        'queue/q/:search': 'search',
        'queue/q/:search/id/:id': 'searchId',
        'queue/q/:search/id/:id/:action': 'searchIdAction',
        'queue/p/:page': 'page',
        'queue/p/:page/id/:id': 'pageId',
        'queue/p/:page/id/:id/:action': 'pageIdAction',
        'queue/q/:search/p/:page': 'searchPage',
        'queue/q/:search/p/:page/id/:id/:action': 'index'
    },

    /**
     * Initialization.
     *
     * @param {App} app The root application object.
     * @param {Object} options Initialization options.
     */
    initialize: function(app, options) {
        CollarRouter.prototype.initialize.call(this, app, options);
        this.controller = this.createController(QueueController, 'queue', this.options);
    }
});