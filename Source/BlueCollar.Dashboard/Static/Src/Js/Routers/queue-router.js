/**
 * Queue area router implementation.
 *
 * @constructor
 * @extends {CollarRouter}
 */
var QueueRouter = CollarRouter.extend({
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
    },

    /**
     * Handles the root #queue route.
     *
     * @param {String} search The requested search string.
     * @param {Number} page The requested page number.
     */
    index: function(search, page) {
        this.controller.index(decodeURIComponent(search || ''), decodeURIComponent(page || '1'));
        this.trigger('nav', this, {name: 'Queue'});
    },

    /**
     * Handles the empty-search paging route.
     *
     * @param {Number} page The requested page number.
     */
    page: function(page) {
        this.index('', page);
    },

    /**
     * Handles the non-paged search route.
     *
     * @param {String} search The requested search string.
     */
    search: function(search) {
        this.index(search, 1);
    }
});