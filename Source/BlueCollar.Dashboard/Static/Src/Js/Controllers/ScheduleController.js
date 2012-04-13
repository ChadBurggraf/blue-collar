/**
 * Controller for Schedule management.
 */
var ScheduleController = BlueCollarController.extend({
    collection: ScheduleList,
    fragment: 'schedules',
    listView: ScheduleListView,

    add: function(listView) {
        var model = new Schedule({StartOn: Date.today()}),
            view = new ScheduleFormView({model: model});

        model.urlRoot = this.urlRoot;
        this.collection.clearEditing();

        this.forms.html(view.render().el);
        view.bind('cancel', this.addEditCancel, this);
        view.bind('submit', this.addSubmit, this);
        view.focus();
    },

    createListView: function() {
        var view = BlueCollarController.prototype.createListView.call(this);
        view.bind('manage', this.manage, this);
        return view;
    },

    addEditCancel: function(view) {
        view.remove();
        this.collection.clearEditing();
    },

    addSubmit: function(view, attributes) {
        this.submit(view, attributes, 'The schedule "' + attributes.Name + '" was created successfully.');
    },

    edit: function(listView, model) {
        var editModel = model.clone(),
            view = new ScheduleFormView({model: editModel});

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
        this.destroy(view, 'The schedule "' + view.model.get('Name') + '" was deleted successfully.');
    },

    editSubmit: function(view, attributes) {
        this.submit(view, attributes, 'The schedule "' + attributes.Name + '" was updated successfully.');
    },

    manage: function(listView, model) {
        var fragment = this.navigateFragment() + '/id' + model.get('Id');
        this.router.navigate(fragment, true);
    }
});