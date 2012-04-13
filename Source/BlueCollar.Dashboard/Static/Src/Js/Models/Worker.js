/**
 * Worker model.
 */
var Worker = BlueCollarModel.extend({
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
 * Worker signal model.
 */
var WorkerSignal = BlueCollarModel.extend({
    defaults: {
        'Id': 0,
        'MachineAddress': '',
        'MachineName': '',
        'Name': null,
        'Signal': 'None'
    },

    url: function() {
        return BlueCollarModel.prototype.url.call(this).appendUrlPath('signal');
    }
});

/**
 * Worker collection.
 */
var WorkerList = BlueCollarCollection.extend({
    model: Worker
});