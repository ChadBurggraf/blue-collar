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
        this.view.bind('details', this.details, this);
        this.view.bind('fetch', this.fetch, this);
        this.view.bind('editDelete', this.editDelete, this);
        this.view.bind('editSubmit', this.editSubmit, this);
    },

    /**
     * Handles a success response from the server.
     *
     * @param {Object} args The original event arguments that initiated the server action.
     * @param {CollarModel} model The model that the server action was taken on behalf of.
     * @param {jqXHR} response The response received from the server.
     */
    success: function(args, model, response) {
        var name = model.get('JobName') || model.get('JobType');
        CollarController.prototype.success.call(this, args, model, response);

        NoticeView.create({
            className: 'alert-success',
            model: {Title: 'Success!', Message: 'The job ' + name + ' was ' + args.Action + ' successfully.'}
        });
    }
});