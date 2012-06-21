/**
 * Models a history entry.
 *
 * @constructor
 */
var HistoryModel = CollarModel.extend({
    defaults: {
        'Id': 0,
        'FinishedOn': null,
        'JobName': null,
        'JobType': null,
        'QueueName': null,
        'ScheduleName': null,
        'StartedOn': null,
        'Status': 'None',
        'TryNumber': 0
    }
});

/**
 * Models the full details of a history entry.
 *
 * @constructor
 */
var HistoryDetailsModel = CollarModel.extend({
    defaults: {
        'Id': null,
        'Data': null,
        'Exception': null,
        'FinishedOn': null,
        'JobName': null,
        'JobType': null,
        'QueuedOn': null,
        'QueueName': null,
        'ScheduleName': null,
        'StartedOn': null,
        'Status': 'None',
        'TryNumber': 0,
        'WorkerMachineAddress': null,
        'WorkerMachineName': null,
        'WorkerName': null
    }
});

/**
 * Represents a collection of {HistoryModel}s.
 *
 * @constructor
 */
var HistoryCollection = CollarCollection.extend({
    model: HistoryModel
});