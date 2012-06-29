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
        this.view.bind('edit', this.navigate, this);
        this.view.bind('editCancel', this.navigate, this);
        this.view.bind('editSubmit', this.editSubmit, this);
    },

    /**
     * Handle's this instance's view's editDelete event.
     *
     * @param {Object} sender The event sender.
     * @param {Object} args The event arguments.
     */
    editDelete: function(sender, args) {
        args.Model.destroy({
            error: _.bind(this.error, this, args.Model)
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
            error: _.bind(this.error, this, args.Model)
        });

        this.navigate();
    },

    /**
     * Handles an error response from the server.
     *
     * @param {CollarModel} model The model that caused the error.
     * @param {jqXHR} response The response received from the server.
     */
    error: function(model, response) {
        
    },

    /**
     * Handles this instance's collection's reset event.
     */
    reset: function() {
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
     * Handles a success response from the server.
     *
     * @param {CollarModel} model The model that was saved.
     * @param {jqXHR} response The response received from the server.
     */
    success: function(model, response) {
        NoticeView.create({
            className: 'alert-success',
            model: {Title: 'Success!', Message: 'The worker ' + model.get('Name') + ' was saved successfully.'}
        });
    }
});