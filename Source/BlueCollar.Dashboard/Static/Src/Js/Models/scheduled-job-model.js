/**
 * Models a scheduled job.
 *
 * @constructor
 */
var ScheduledJobModel = CollarModel.extend({
    defaults: {
        'Id': 0,
        'JobType': null,
        'Data': '{}'
    },
    fragment: 'schedules',

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
 * Represents a collection of {ScheduledJobModel}s.
 *
 * @constructor
 */
var ScheduledJobCollection = CollarCollection.extend({
    fragment: 'schedules',
    model: ScheduledJobModel,

    /**
     * Triggers the area event for this instance, if the givem models object area information.
     *
     * @param {Object} models The models object being used to reset this instance.
     */
    triggerArea: function(models) {
        if (models.Name || models.PageCount || models.PageNumber || models.TotalCount) {
            this.trigger('area', this, {
                ScheduleName: models.Name, 
                PageCount: models.PageCount, 
                PageNumber: models.PageNumber, 
                TotalCount: models.TotalCount
            });
        }
    }
});