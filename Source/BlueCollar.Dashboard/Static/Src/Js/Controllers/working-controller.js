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
        this.view.bind('details', this.details, this);
        this.view.bind('fetch', this.fetch, this);
        this.view.bind('signalSubmit', this.signalSubmit, this);
    },

    /**
     * Handles a success response from the server.
     *
     * @param {Object} args The original event arguments that initiated the server action.
     * @param {CollarModel} model The model that the server action was taken on behalf of.
     * @param {jqXHR} response The response received from the server.
     */
    success: function(args, model, response) {
        CollarController.prototype.success.call(this, args, model, response);
        
        if (args.Action === 'signalled') {
            model = this.model.get('Collection').find(function(m) { return m.get('Id') === args.Model.get('Id'); });
        }

        NoticeView.create({
            className: 'alert-success',
            model: {Title: 'Success!', Message: 'The job ' + model.get('Name') + ' was ' + args.Action + ' successfully.'}
        });
    }
});