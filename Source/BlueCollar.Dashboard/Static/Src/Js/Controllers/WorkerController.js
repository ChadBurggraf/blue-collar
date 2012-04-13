/**
 * Controller for Worker management.
 */
var WorkerController = BlueCollarController.extend({
    collection: WorkerList,
    fragment: 'workers',
    listView: WorkerListView,

    initialize: function() {
        this.machines = [];
    },

    add: function(listView) {
        var model = new Worker(),
            view = new WorkerFormView({model: model, machines: this.machines});

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
        this.submit(view, attributes, 'The worker "' + attributes.Name + '" was created successfully.');
    },

    createListView: function() {
        var view = BlueCollarController.prototype.createListView.call(this);
        view.bind('signal', this.signal, this);
        return view;
    },

    edit: function(listView, model) {
        var editModel = model.clone(),
            view = new WorkerFormView({model: editModel, machines: this.machines});

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
        this.destroy(view, 'The worker "' + view.model.get('Name') + '" was deleted successfully.');
    },

    editSubmit: function(view, attributes) {
        this.submit(view, attributes, 'The worker "' + attributes.Name + '" was updated successfully.');
    },

    reset: function() {
        var lookup = {},
            worker,
            name,
            address,
            key,
            machine,
            i,
            n;

        BlueCollarController.prototype.reset.call(this);
        this.machines = [];
        
        for (i = 0, n = this.collection.length; i < n; i++) {
            worker = this.collection.at(i);
            name = worker.get('MachineName');
            address = worker.get('MachineAddress');
            key = (name || '' + address || '').toUpperCase();
            machine = {Name: name, Address: address};

            if (key) {
                if (_.isUndefined(lookup[key])) {
                    lookup[key] = [machine];
                    this.machines.push(machine);
                } else if (!_.any(lookup[key], function(m) { return m.Name.toUpperCase() === name.toUpperCase() || m.Address.toUpperCase() === address.toUpperCase(); })) {
                    lookup[key].push(machine);
                    this.machines.push(machine);
                }
            }
        }
    },

    signal: function(listView, model) {
        var signalModel = new WorkerSignal({
                Id: model.get('Id'), 
                MachineAddress: model.get('MachineAddress'),
                MachineName: model.get('MachineName'),
                Name: model.get('Name'), 
                Signal: model.get('Signal')
            }),
            view = new WorkerSignalFormView({model: signalModel});
        
        signalModel.urlRoot = this.urlRoot;
        var url = signalModel.url();

        this.collection.clearEditing();
        model.set({editing: true});

        this.forms.html(view.render().el);
        view.bind('cancel', this.addEditCancel, this);
        view.bind('submit', this.signalSubmit, this);
        view.focus();
    },

    signalSubmit: function(view, attributes) {
        this.submit(view, attributes, 'The worker "' + view.model.get('Name') + '" was signaled successfully.');
    }
});