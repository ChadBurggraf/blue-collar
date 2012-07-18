/**
 * Manages the root history view.
 *
 * @constructor
 * @extends {AreaView}
 */
var HistoryView = AreaView.extend({
    template: _.template($('#history-template').html()),

    /**
     * Initialization.
     *
     * @param {Object} options Initialization options.
     */
    initialize: function(options) {
        AreaView.prototype.initialize.call(this, options);
        this.listView = new HistoryListView({model: this.model});
        this.listView.bind('display', this.display, this);
        this.listView.bind('signal', this.signal, this);
    },

    /**
     * Handle's the display view's enqueue event.
     *
     * @param {Object} sender The event sender.
     * @param {Object} args The event arguments.
     */
    enqueue: function(sender, args) {
        var model = new QueueModel(args.Model.attributes, {jsonUrlRoot: this.model.jsonUrlRoot}),
            view = new HistoryReEnqueueView({model: model}),
            callback;

        view.bind('submit', this.enqueueSubmit, this);
        view.bind('cancel', this.enqueueCancel, this);

        this.$('.details').html(view.render().el);
        view.focus();

        if (!args.Model.get('DetailsLoaded')) {
            callback = function() {
                args.Model.unbind('change:Data', callback);
                model.set({Data: args.Model.get('Data')});
            };

            args.Model.bind('change:Data', callback);
            this.trigger('details', this, {Model: args.Model});
        }
    },

    /**
     * Handles the enqueue view's cancel event.
     *
     * @param {Object} sender The event sender.
     * @param {Object} args The event arguments.
     */
    enqueueCancel: function(sender, args) {
        this.renderId();
    },

    /**
     * Handles the enqueue view's submit event.
     *
     * @param {Object} sender The event sender.
     * @param {Object} args The event arguments.
     */
    enqueueSubmit: function(sender, args) {
        sender.showLoading();
        this.trigger('enqueueSubmit', this, _.extend({}, args, {View: sender}));
    },

    /**
     * Renders the ID view for the given model in the given details element.
     *
     * @param {jQuery} el The jQuery object containing the details element to render into.
     * @param {CollarModel} model The model to render the ID view for.
     */
    renderIdView: function(el, model) {
        var view = new HistoryDisplayView({model: model});
        view.bind('enqueue', this.enqueue, this);
        view.bind('cancel', this.displayCancel, this);

        el.html(view.render().el);
        view.focus();

        if (!model.get('DetailsLoaded')) {
            this.trigger('details', this, {Model: model});
        }
    }
});