/**
 * Queue area controller implementation.
 *
 * @constructor
 * @extends {CollarController}
 */
var QueueController = CollarController.extend({
    collection: QueueCollection,
    fragment: 'queue',

    /**
     * Initialization.
     *
     * @param {Object} options Initialization options.
     */
    initialize: function(options) {
        this.view = new QueueView({model: this.model});
        this.view.bind('fetch', this.fetch, this);
    }
});