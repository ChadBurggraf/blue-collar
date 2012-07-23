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
    fragment: 'history',

    /**
     * Parses the model's data as returned by the server.
     *
     * @param {Object} response The raw response object received from the server.
     * @return {Object} The parsed response object.
     */
    parse: function(response) {
        response = CollarModel.prototype.parse.call(this, response);
        return this.parseData(response);
    }
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