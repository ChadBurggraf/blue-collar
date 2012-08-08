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
    fragment: 'queue',

    /**
     * Gets a copy of th emodel's attributes, suitable for editing in a UI.
     *
     * @return {Object} An editable copy of the model's underlying attributes.
     */
    toEditJSON: function() {
        var obj = CollarModel.prototype.toEditJSON.call(this);

        if (obj.Data === '{}') {
            obj.Data = null;
        }

        return obj;
    }
});

/**
 * Represents a collection of {QueueModel}s.
 *
 * @constructor
 */
var QueueCollection = CollarCollection.extend({
    fragment: 'queue',
    model: QueueModel
});