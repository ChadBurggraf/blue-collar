/**
 * Controller for Queued management.
 */
var QueuedController = BlueCollarController.extend({
    collection: QueuedList,
    fragment: 'queue',
    listView: QueuedListView,

    add: function(listView) {
        var model = new Queued(),
            view = new QueuedFormView({model: model});

        model.urlRoot = this.urlRoot;
        this.collection.clearEditing();

        this.forms.html(view.render().el);
        view.bind('cancel', this.addCancel, this);
        view.bind('submit', this.addSubmit, this);
        view.focus();
    },

    addCancel: function(view) {
        view.remove();
    },

    addSubmit: function(view, attributes) {
        this.submit(view, attributes, 'A job of typ "' + attributes.JobType + '" was enqueued successfully.');
    },

    createListView: function() {
        var view = BlueCollarController.prototype.createListView.call(this);
        view.bind('display', this.display, this);
        return view;
    },

    del: function(view) {
        this.destroy(view, 'The queued job "' + view.model.get('JobName') + '" was deleted successfully.');
    },

    display: function(listView, model) {
        var view,
            data = model.get('Data');

        if (!model.get('Data')) {
            model.fetch({error: _.bind(this.ajaxError, this)});
        }

        var view = new QueuedDetailsView({model: model});

        this.collection.clearEditing();
        model.set({editing: true});

        this.forms.html(view.render().el);
        view.bind('submit', this.done, this);
        view.bind('delete', this.del, this);

        scrollTo(0, 0);
    },

    done: function(view, attributes) {
        this.collection.clearEditing();
        view.remove();
    }
});