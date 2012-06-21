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
        this.controller.index(search, page);
        this.trigger('nav', this, {name: 'History'});
    }
});