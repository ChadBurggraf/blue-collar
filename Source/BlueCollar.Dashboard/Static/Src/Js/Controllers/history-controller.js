/**
 * History area controller implementation.
 *
 * @constructor
 * @extends {CollarController}
 */
var HistoryController = CollarController.extend({
    collection: HistoryCollection,
    fragment: 'history',

    /**
     * Initialization.
     *
     * @param {Object} options Initialization options.
     */
    initialize: function(options) {
        this.view = new HistoryView({el: this.page, model: this.model});
        this.view.bind('fetch', this.fetch, this);
    },

    /**
     * Renders the index view.
     *
     * @param {String} search The search string to filter the view on.
     * @param {Number} page The page number to filter the view on.
     */
    index: function(search, page) {
        this.model.set({Search: search || '', PageNumber: page || 1, Loading: true}, {silent: true});
        this.view.render();
        this.fetch();
    }
});