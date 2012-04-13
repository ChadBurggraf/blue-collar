/**
 * Schedule model.
 */
var Schedule = BlueCollarModel.extend({
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
 * Schedule collection.
 */
var ScheduleList = BlueCollarCollection.extend({
    model: Schedule
});