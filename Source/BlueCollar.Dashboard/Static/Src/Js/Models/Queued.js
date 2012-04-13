/**
 * Queued model.
 */
var Queued = BlueCollarModel.extend({
    defaults: {
        'Id': 0,
        'Data': null,
        'JobName': null,
        'JobType': null,
        'QueueName': null,
        'ScheduleName': null,
        'QueuedOn': null,
        'TryNumber': 0
    }
});

/**
 * Queued collection.
 */
var QueuedList = BlueCollarCollection.extend({
    model: Queued
});