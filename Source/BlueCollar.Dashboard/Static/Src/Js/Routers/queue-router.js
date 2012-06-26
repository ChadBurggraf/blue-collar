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
        'queue/:search/p:page': 'index',
        'queue//p:page': 'page',
        'queue/*search': 'search'
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