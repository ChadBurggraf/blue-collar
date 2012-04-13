/**
 * ScheduledJob model.
 */
var ScheduledJob = BlueCollarModel.extend({
    defaults: {
        'Id': 0,
        'JobType': null,
        'Properties': '{}'
    }
});

/**
 * ScheduledJob collection.
 */
var ScheduledJobList = BlueCollarCollection.extend({
    model: ScheduledJob,

    reset: function(models, options) {
        models = models || {};
        this.ScheduleId = models.Id;
        this.ScheduleName = models.Name;
        return BlueCollarCollection.prototype.reset.call(this, models, options);
    }
});