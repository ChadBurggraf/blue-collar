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
    },

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
 * Represents a collection of {QueueModel}s.
 *
 * @constructor
 */
var QueueCollection = CollarCollection.extend({
    model: QueueModel
});