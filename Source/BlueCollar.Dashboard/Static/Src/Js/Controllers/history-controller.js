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
        this.view = new HistoryView({model: this.model});
        this.view.bind('details', this.details, this);
        this.view.bind('fetch', this.fetch, this);
    }
});