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
     * Initialization.
     *
     * @param {Object} app A set of initial model attribute values.
     * @param {Object} options Initialization options.
     */
    initialize: function(attributes, options) {
        CollarModel.prototype.initialize.call(this, attributes, options);
        this.setScheduleId(options.scheduleId);
    },

    /**
     * Sets this instance's schedule ID.
     *
     * @param {Number} scheduleId The schedule ID value to set.
     */
    setScheduleId: function(scheduleId) {
        this.scheduleId = this.options.scheduleId = scheduleId;
    },

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
    },

    /**
     * Gets the URL root to use when interacting with the model on the server.
     *
     * @return {String} The model's server URL root.
     */
    urlRoot: function() {
        if (!this.scheduleId) {
            throw new Error('scheduleId must be set to a value.');
        }

        return CollarModel.prototype.urlRoot.call(this).appendUrlPath(this.scheduleId).appendUrlPath('jobs');
    }
});

/**
 * Models a set of scheduled job orderings.
 *
 * @constructor
 */
var ScheduledJobsOrderModel = CollarModel.extend({
    defaults: {
        'Numbers': []
    },
    fragment: 'schedules',

    /**
     * Initialization.
     *
     * @param {Object} app A set of initial model attribute values.
     * @param {Object} options Initialization options.
     */
    initialize: function(attributes, options) {
        CollarModel.prototype.initialize.call(this, attributes, options);
        this.setScheduleId(options.scheduleId);
    },

    /**
     * Gets a value indicating whether this instance represents a new model.
     *
     * @return {boolean} True if the model is new, false otherwise.
     */
    isNew: function() {
        return false;
    },

    /**
     * Sets this instance's schedule ID.
     *
     * @param {Number} scheduleId The schedule ID value to set.
     */
    setScheduleId: function(scheduleId) {
        this.scheduleId = this.options.scheduleId = scheduleId;
    },

    /**
     * Gets the URL root to use when interacting with the model on the server.
     *
     * @return {String} The model's server URL root.
     */
    urlRoot: function() {
        if (!this.scheduleId) {
            throw new Error('scheduleId must be set to a value.');
        }

        return CollarModel.prototype.urlRoot.call(this).appendUrlPath(this.scheduleId).appendUrlPath('jobs/order');
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
     * Initialization.
     *
     * @param {Object} models An object specifying the model collection.
     * @param {Object} options Initialization options.
     */
    initialize: function(models, options) {
        CollarCollection.prototype.initialize.call(this, models, options);
        this.setScheduleId(options.scheduleId);
    },

    /**
     * Performs a fetch opteration on this collection.
     *
     * @param {Object} options The fetch options to use.
     * @return {jqXHR} The XHR object used to perform the fetch.
     */
    fetch: function(options) {
        return CollarCollection.prototype.fetch.call(this, _.extend({scheduleId: this.scheduleId}, options));
    },

    /**
     * Replaces this instance's model collection with the given collection.
     *
     * @param {Object} models An object specifying the new model collection.
     * @param {Object} options The options to use when performing the reset.
     * @return {CollarCollection} This instance.
     */
    reset: function(models, options) {
        return CollarCollection.prototype.reset.call(this, models, _.extend({scheduleId: this.scheduleId}, options));
    },

    /**
     * Sets this instance's schedule ID.
     *
     * @param {Number} scheduleId The schedule ID value to set.
     */
    setScheduleId: function(scheduleId) {
        this.scheduleId = this.options.scheduleId = scheduleId;
        this.each(function(m) { m.setScheduleId(scheduleId); });
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
    },

    /**
     * Gets the URL root to use when interacting with the collection on the server.
     *
     * @return {String} The collection's server URL root.
     */
    urlRoot: function() {
        if (!this.scheduleId) {
            throw new Error('scheduleId must be set to a value.');
        }

        return CollarCollection.prototype.urlRoot.call(this).appendUrlPath(this.scheduleId).appendUrlPath('jobs');
    }
});