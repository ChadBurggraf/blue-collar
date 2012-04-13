/**
 * Working model.
 */
var Working = BlueCollarModel.extend({
    defaults: {
        'Id': 0,
        'ScheduleName': null,
        'QueueName': null,
        'JobName': null,
        'JobType': null,
        'Data': null,
        'QueuedOn': null,
        'TryNumber': 0,
        'StartedOn': null,
        'Signal': 'None',
        'WorkerMachineAddress': null,
        'WorkerMachineName': null,
        'WorkerName': null
    }
});

/**
 * Working signal model.
 */
var WorkingSignal = BlueCollarModel.extend({
    defaults: {
        'Id': 0,
        'JobName': null,
        'Signal': 'None',
        'WorkerName': null
    },

    url: function() {
        return BlueCollarModel.prototype.url.call(this).appendUrlPath('signal');
    }
});

/**
 * Working collection.
 */
var WorkingList = BlueCollarCollection.extend({
    model: Working
});