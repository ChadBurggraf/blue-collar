/**
 * Models a queue entry.
 *
 * @constructor
 */
var QueueModel = CollarModel.extend({
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
 * Represents a collection of {QueueModel}s.
 *
 * @constructor
 */
var QueueCollection = CollarCollection.extend({
    model: QueueModel
});