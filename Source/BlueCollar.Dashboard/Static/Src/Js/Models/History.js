/**
 * History model.
 */
var History = BlueCollarModel.extend({
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

/*
 * History details model.
 */
var HistoryDetails = BlueCollarModel.extend({
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

/*
 * History collection.
 */
var HistoryList = BlueCollarCollection.extend({
    model: History
});