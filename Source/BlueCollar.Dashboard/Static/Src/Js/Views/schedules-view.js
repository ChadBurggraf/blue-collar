/**
 * Manages the root schedules view.
 *
 * @constructor
 * @extends {AreaView}
 */
var SchedulesView = AreaView.extend({
    template: _.template($('#schedules-template').html()),

    /**
     * Initialization.
     *
     * @param {Object} options Initialization options.
     */
    initialize: function(options) {
        AreaView.prototype.initialize.call(this, options);
        this.listView = new SchedulesListView({model: this.model});
        this.listView.bind('edit', this.edit, this);
    },

    /**
     * Handle's the add button's click event.
     */
    add: function() {
        var model = new ScheduleModel({StartOn: Date.today()}, {jsonUrlRoot: this.model.jsonUrlRoot});
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
        var view = new SchedulesEditView({model: model});
        view.bind('cancel', this.editCancel, this);
        view.bind('delete', this.editDelete, this);
        view.bind('submit', this.editSubmit, this);
        el.html(view.render().el);
        view.focus();
    }
});