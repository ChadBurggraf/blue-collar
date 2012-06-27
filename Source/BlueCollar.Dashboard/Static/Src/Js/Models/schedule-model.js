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