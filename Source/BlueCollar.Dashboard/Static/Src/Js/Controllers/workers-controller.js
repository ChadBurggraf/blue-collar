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
    }
});