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
    }
});

/**
 * Represents a collection of {ScheduledJobModel}s.
 *
 * @constructor
 */
var ScheduledJobCollection = CollarCollection.extend({
    model: ScheduledJobModel,

    /**
     * Sets this instance's urlRoot property.
     *
     * @param {String} urlRoot The value to set.
     */
    setUrlRoot: function(urlRoot) {
        CollarCollection.prototype.setUrlRoot.call(this, urlRoot);

        this.each(function(model) {
            model.urlRoot = urlRoot;
        });
    },

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