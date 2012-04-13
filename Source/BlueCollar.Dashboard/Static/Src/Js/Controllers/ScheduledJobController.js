/**
 * Controller for ScheduledJob management.
 */
var ScheduledJobController = BlueCollarController.extend({
    collection: ScheduledJobList,
    fragment: 'schedules/id',
    listView: ScheduledJobListView,

    initialize: function(options) {
        this.scheduleId = options.scheduleId;
    },

    add: function(listView) {
        var model = new ScheduledJob(),
            view = new ScheduledJobFormView({model: model});

        model.urlRoot = this.urlRoot;
        this.collection.clearEditing();

        this.forms.html(view.render().el);
        view.bind('cancel', this.addEditCancel, this);
        view.bind('submit', this.addSubmit, this);
        view.focus();
    },

    addEditCancel: function(view) {
        view.remove();
        this.collection.clearEditing();
    },

    addSubmit: function(view, attributes) {
        this.submit(view, attributes, 'The job "' + attributes.JobType + '" was created successfully.');
    },

    edit: function(listView, model) {
        var editModel = model.clone(),
            view = new ScheduledJobFormView({model: editModel});

        editModel.urlRoot = this.urlRoot;

        this.collection.clearEditing();
        model.set({editing: true});

        this.forms.html(view.render().el);
        view.bind('cancel', this.addEditCancel, this);
        view.bind('submit', this.editSubmit, this);
        view.bind('delete', this.editDelete, this);
        view.focus();
    },

    editDelete: function(view) {
        this.destroy(view, 'The job "' + view.model.get('JobType') + '" was delete successfully.');
    },

    editSubmit: function(view, attributes) {
        this.submit(view, attributes, 'The job "' + attributes.JobType + '" was updated successfully.');
    },

    navigateFragment: function() {
        return this.fragment + this.scheduleId;
    },

    submit: function(view, attributes, successMessage) {
        BlueCollarController.prototype.submit.call(
            this,
            view,
            _.extend(attributes, {ScheduleId: this.scheduleId}),
            successMessage);
    }
});