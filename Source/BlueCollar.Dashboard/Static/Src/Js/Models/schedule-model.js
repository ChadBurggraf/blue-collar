/**
 * Models a schedule.
 *
 * @constructor
 */
var ScheduleModel = CollarModel.extend({
    defaults: {
        'Id': 0,
        'QueueName': null,
        'Name': null,
        'StartOn': null,
        'EndOn': null,
        'RepeatType': 'None',
        'RepeatValue': null,
        'Enabled': true
    },

    /**
     * Gets a copy of the model's attributes.
     *
     * @return {Object} A copy of the model's underlying attributes.
     */
    toJSON: function() {
        return _.extend({}, CollarModel.prototype.toJSON.call(this), {
            ManageUrl: (this.fragment || '') + '/id/' + encodeURIComponent(this.get('Id').toString()) + '/jobs'
        });
    }
});

/**
 * Represents a collection of {ScheduleModel}s.
 *
 * @constructor
 */
var ScheduleCollection = CollarCollection.extend({
    model: ScheduleModel
});