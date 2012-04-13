/**
 * Controller for Working management.
 */
var WorkingController = BlueCollarController.extend({
    collection: WorkingList,
    fragment: 'working',
    listView: WorkingListView,

    createListView: function() {
        var view = BlueCollarController.prototype.createListView.call(this);
        view.bind('display', this.display, this);
        view.bind('signal', this.signal, this);
        return view;
    },

    display: function(listView, model) {
        var view,
            data = model.get('Data');

        if (!model.get('Data')) {
            model.fetch({error: _.bind(this.ajaxError, this)});
        }

        var view = new WorkingDetailsView({model: model});

        this.collection.clearEditing();
        model.set({editing: true});

        this.forms.html(view.render().el);
        view.bind('submit', this.done, this);

        scrollTo(0, 0);
    },

    done: function(view, attributes) {
        this.collection.clearEditing();
        view.remove();
    },

    signal: function(listView, model) {
        var signalModel = new WorkingSignal({
                Id: model.get('Id'), 
                JobName: model.get('JobName'),
                Signal: model.get('Signal'),
                WorkerName: model.get('WorkerName')
            }),
            view = new WorkingSignalFormView({model: signalModel});
        
        signalModel.urlRoot = this.urlRoot;
        var url = signalModel.url();

        this.collection.clearEditing();
        model.set({editing: true});

        this.forms.html(view.render().el);
        view.bind('cancel', this.signalCancel, this);
        view.bind('submit', this.signalSubmit, this);
        view.focus();
    },

    signalCancel: function(view) {
        view.remove();
        this.collection.clearEditing();
    },

    signalSubmit: function(view, attributes) {
        this.submit(view, attributes, 'The job "' + view.model.get('JobName') + '" was signaled successfully.');
    }
});