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

        this.model.get('Collection').bind('reset', this.reset, this);
        
        this.view = new WorkersView({el: this.page, model: this.model, machines: this.machines});
        this.view.bind('fetch', this.fetch, this);
        this.view.bind('editDelete', this.editDelete, this);
        this.view.bind('editSubmit', this.editSubmit, this);
        this.view.bind('signalSubmit', this.signalSubmit, this);
    },

    /**
     * Handle's this instance's view's editDelete event.
     *
     * @param {Object} sender The event sender.
     * @param {Object} args The event arguments.
     */
    editDelete: function(sender, args) {
        args.Model.destroy({
            success: _.bind(this.success, this, args),
            error: _.bind(this.error, this, args)
        });
    },

    /**
     * Handle's this instance's view's editSubmit event.
     *
     * @param {Object} sender The event sender.
     * @param {Object} args The event arguments.
     */
    editSubmit: function(sender, args) {
        args.Model.save(args.Attributes, {
            success: _.bind(this.success, this, args),
            error: _.bind(this.error, this, args),
            wait: true
        });
    },

    /**
     * Refreshes this instance's machine list.
     */
    refreshMachines: function() {
        var collection = this.model.get('Collection'),
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
     * Handle's this instance's view's signalSubmit event.
     *
     * @param {Object} sender The event sender.
     * @param {Object} args The event arguments.
     */
    signalSubmit: function(sender, args) {
        args.Model.save(args.Attributes, {
            success: _.bind(this.success, this, args),
            error: _.bind(this.error, this, args),
            wait: true
        });
    },

    /**
     * Handles a success response from the server.
     *
     * @param {Object} args The original event arguments that initiated the server action.
     * @param {CollarModel} model The model that the server action was taken on behalf of.
     * @param {jqXHR} response The response received from the server.
     */
    success: function(args, model, response) {
        if (args.Action === 'updated') {
            this.refreshMachines();
        }

        CollarController.prototype.success.call(this, args, model, response);
    }
});