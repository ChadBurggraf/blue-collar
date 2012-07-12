/**
 * Manages the root workers view.
 *
 * @constructor
 * @extends {AreaView}
 */
var WorkersView = AreaView.extend({
    template: _.template($('#workers-template').html()),

    /**
     * Initialization.
     *
     * @param {Object} options Initialization options.
     */
    initialize: function(options) {
        AreaView.prototype.initialize.call(this, options);
        this.machines = options.machines || [];
        this.listView = new WorkersListView({model: this.model});
        this.listView.bind('edit', this.edit, this);
        this.listView.bind('signal', this.signal, this);
    },

    /**
     * Handle's the add button's click event.
     */
    add: function() {
        var model = new WorkerModel();
        model.urlRoot = this.model.get('UrlRoot');
        this.model.clearId();
        this.renderIdView($('.details'), model);
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
                signalModel = new WorkerSignalModel(model.attributes);
                signalModel.urlRoot = this.model.get('UrlRoot');
                view = new WorkersSignalView({model: signalModel});
                view.bind('cancel', this.editCancel, this);
                view.bind('submit', this.signalSubmit, this);
                render = true;
            } else {
                this.model.set({Id: 0, Action: ''});
            }
        } else {
            view = new WorkersEditView({model: model, machines: this.machines});
            view.bind('cancel', this.editCancel, this);
            view.bind('delete', this.editDelete, this);
            view.bind('submit', this.editSubmit, this);
            render = true;
        }

        if (render) {
            el.html(view.render().el);
            view.focus();
        }
    }
});