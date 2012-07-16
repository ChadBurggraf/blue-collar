/**
 * Manages the root working jobs view.
 *
 * @constructor
 * @extends {AreaView}
 */
var WorkingView = AreaView.extend({
    template: _.template($('#working-template').html()),

    /**
     * Initialization.
     *
     * @param {Object} options Initialization options.
     */
    initialize: function(options) {
        AreaView.prototype.initialize.call(this, options);

        this.listView = new WorkingListView({model: this.model});
        this.listView.bind('display', this.display, this);
        this.listView.bind('signal', this.signal, this);
    },

    /**
     * Renders the ID view for the given model in the given details element.
     *
     * @param {jQuery} el The jQuery object containing the details element to render into.
     * @param {CollarModel} model The model to render the ID view for.
     */
    renderIdView: function(el, model) {
        var render = false,
            view,
            signalModel;

        if (this.model.get('Action') === 'signal') {
            if (model.get('Signal') === 'None') {
                signalModel = new WorkingSignalModel(model.attributes, {jsonUrlRoot: this.model.jsonUrlRoot});
                view = new WorkingSignalView({model: signalModel});
                view.bind('cancel', this.editCancel, this);
                view.bind('submit', this.signalSubmit, this);
                render = true;
            } else {
                this.model.set({Id: 0, Action: ''});
            }
        } else {
            view = new WorkingDisplayView({model: model});
            view.bind('cancel', this.displayCancel, this);
            render = true;
        }

        if (render) {
            el.html(view.render().el);
            view.focus();

            if (!model.get('DetailsLoaded')) {
                this.trigger('details', this, {Model: model});
            }
        }
    }
});