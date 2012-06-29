/**
 * Manages the root workers view.
 *
 * @constructor
 * @extends {AreaView}
 */
var WorkersView = AreaView.extend({
    events: {
        'click button.btn-add': 'add'
    },
    template: _.template($('#workers-template').html()),

    /**
     * Initialization.
     *
     * @param {Object} options Initialization options.
     */
    initialize: function(options) {
        AreaView.prototype.initialize.call(this, options);

        this.machines = options.machines || [];
        this.model.get('Collection').bind('reset', this.renderId, this);

        this.listView = new WorkersListView({model: this.model});
        this.listView.bind('edit', this.edit, this);
    },

    /**
     * Handle's the add button's click event.
     */
    add: function() {
        var model = new WorkerModel();
        model.urlRoot = this.model.get('UrlRoot');
        this.model.set({Id: 0});
        this.renderIdView($('.details'), model);
    },

    /**
     * Renders the ID view for the given model in the given details element.
     *
     * @param {jQuery} el The jQuery object containing the details element to render into.
     * @param {CollarModel} model The model to render the ID view for.
     */
    renderIdView: function(el, model) {
        var view = new WorkersEditView({model: model, machines: this.machines});
        view.bind('cancel', this.editCancel, this);
        view.bind('delete', this.editDelete, this);
        view.bind('submit', this.editSubmit, this);
        el.html(view.render().el);
        view.focus();
    }
});