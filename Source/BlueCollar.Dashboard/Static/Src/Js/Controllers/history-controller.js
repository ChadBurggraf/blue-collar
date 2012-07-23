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
        this.view.bind('enqueueSubmit', this.enqueueSubmit, this);
        this.view.bind('fetch', this.fetch, this);
    },

    /**
     * Handle's this instance's view's enqueueSubmit event.
     *
     * @param {Object} sender The event sender.
     * @param {Object} args The event arguments.
     */
    enqueueSubmit: function(sender, args) {
        var model = new QueueModel(args.Attributes, {jsonUrlRoot: this.jsonUrlRoot});

        args.Model.save(args.Attributes, {
            success: _.bind(this.success, this, args),
            error: _.bind(this.error, this, args),
            wait: true
        });
    },

    /**
     * Handles a success response from the server.
     *
     * @param {Object} args The original event arguments that initiated the server action.
     * @param {CollarModel} model The model that the server action was taken on behalf of.
     * @param {jqXHR} response The response received from the server.
     */
    success: function(args, model, response) {
        var countsModel = new CountsModel({}, {jsonUrlRoot: this.jsonUrlRoot}),
            countsTrigger = _.bind(this.counts, this);

        CollarController.prototype.success.call(this, args, model, response);

        NoticeView.create({
            className: 'alert-success',
            model: {Title: 'Success!', Message: 'The job ' + model.get('JobType') + ' was ' + args.Action + ' successfully.'}
        });

        if (args.Action === 're-enqueued') {
            countsModel.fetch({
                success: _.bind(function() {
                    countsTrigger(this, {Counts: countsModel.attributes});
                }, this)
            });
        }
    }
});