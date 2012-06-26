/**
 * History area router implementation.
 *
 * @constructor
 * @extends {CollarRouter}
 */
var HistoryRouter = CollarRouter.extend({
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
    },

    /**
     * Handles the root #history route.
     *
     * @param {String} search The requested search string.
     * @param {Number} page The requested page number.
     */
    index: function(search, page) {
        this.controller.index(decodeURIComponent(search || ''), decodeURIComponent(page || '1'));
        this.trigger('nav', this, {name: 'History'});
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