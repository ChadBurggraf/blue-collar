/**
 * Workers area controller implementation.
 *
 * @constructor
 * @extends {CollarController}
 */
var WorkersController = CollarController.extend({
    collection: WorkerCollection,
    fragment: 'workers',

    /**
     * Initialization.
     *
     * @param {Object} options Initialization options.
     */
    initialize: function(options) {
        this.machines = [];

        this.getCollection().bind('reset', this.reset, this);
        
        this.view = new WorkersView({model: this.model, machines: this.machines});
        this.view.bind('fetch', this.fetch, this);
        this.view.bind('editDelete', this.editDelete, this);
        this.view.bind('editSubmit', this.editSubmit, this);
        this.view.bind('signalSubmit', this.signalSubmit, this);
    },

    /**
     * Refreshes this instance's machine list.
     */
    refreshMachines: function() {
        var collection = this.getCollection(),
            lookup = {},
            worker,
            name,
            address,
            key,
            machine,
            i,
            n;

        this.machines = [];

        for (i = 0, n = collection.length; i < n; i++) {
            worker = collection.at(i);
            name = worker.get('MachineName');
            address = worker.get('MachineAddress') || '';
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

        this.view.machines = this.machines = _.sortBy(this.machines, 'Name');
    },

    /**
     * Handles this instance's collection's reset event.
     */
    reset: function() {
        this.refreshMachines();
    },

    /**
     * Handles a success response from the server.
     *
     * @param {Object} args The original event arguments that initiated the server action.
     * @param {CollarModel} model The model that the server action was taken on behalf of.
     * @param {jqXHR} response The response received from the server.
     */
    success: function(args, model, response) {
        CollarController.prototype.success.call(this, args, model, response);
        
        if (args.Action === 'signalled') {
            model = this.getCollection().find(function(m) { return m.get('Id') === args.Model.get('Id'); });
        } else if (args.Action === 'updated') {
            this.refreshMachines();
        }

        NoticeView.create({
            className: 'alert-success',
            model: {Title: 'Success!', Message: 'The worker ' + model.get('Name') + ' was ' + args.Action + ' successfully.'}
        });
    }
});