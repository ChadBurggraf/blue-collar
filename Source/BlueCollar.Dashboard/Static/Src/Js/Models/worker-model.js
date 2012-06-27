/**
 * Models a worker.
 *
 * @constructor
 */
var WorkerModel = CollarModel.extend({
    defaults: {
        'Id': 0,
        'MachineAddress': '',
        'MachineName': '',
        'Name': null,
        'QueueNames': null,
        'Startup': 'Automatic'
    }
});

/**
 * Models a worker signal.
 *
 * @constructor
 */
var WorkerSignalModel = CollarModel.extend({
    defaults: {
        'Id': 0,
        'MachineAddress': '',
        'MachineName': '',
        'Name': null,
        'Signal': 'None'
    },

    /**
     * Gets the URL to use when interacting with the model on the server.
     *
     * @return {String} The model's server URL.
     */
    url: function() {
        return CollarModel.prototype.url.call(this).appendUrlPath('signal');
    }
});

/**
 * Represents a collection of {WorkerModel}s.
 *
 * @constructor
 */
var WorkerCollection = CollarCollection.extend({
    model: WorkerModel
});