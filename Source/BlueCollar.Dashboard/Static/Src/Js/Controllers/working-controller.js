/**
 * Working area controller implementation.
 *
 * @constructor
 * @extends {CollarController}
 */
var WorkingController = CollarController.extend({
    collection: WorkingCollection,
    fragment: 'working',

    /**
     * Initialization.
     *
     * @param {Object} options Initialization options.
     */
    initialize: function(options) {
        this.view = new WorkingView({model: this.model});
        this.view.bind('fetch', this.fetch, this);
    }
});