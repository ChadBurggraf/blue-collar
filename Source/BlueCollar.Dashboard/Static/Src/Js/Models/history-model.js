/**
 * Models a history entry.
 *
 * @constructor
 */
var HistoryModel = CollarModel.extend({
    defaults: {
        'Id': 0,
        'Data': null,
        'Exception': null,
        'FinishedOn': null,
        'JobName': null,
        'JobType': null,
        'QueueName': null,
        'QueuedOn': null,
        'ScheduleName': null,
        'StartedOn': null,
        'Status': 'None',
        'TryNumber': 0,
        'WorkerMachineAddress': null,
        'WorkerMachineName': null,
        'WorkerName': null
    },
    fragment: 'history'
});

/**
 * Represents a collection of {HistoryModel}s.
 *
 * @constructor
 */
var HistoryCollection = CollarCollection.extend({
    fragment: 'history',
    model: HistoryModel
});