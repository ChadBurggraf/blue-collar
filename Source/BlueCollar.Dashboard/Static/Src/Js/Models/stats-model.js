/**
 * Models a set of simple counts.
 *
 * @constructor
 */
var CountsModel = CollarModel.extend({
    defaults: {
        'HistoryCount': 0,
        'QueueCount': 0,
        'ScheduleCount': 0,
        'WorkingCount': 0,
        'WorkerCount': 0
    },
    fragment: 'counts'
});

/**
 * Models a set of history status counts.
 *
 * @constructor
 */
var HistoryStatusCountsModel = CollarModel.extend({
    defaults: {
        'CanceledCount': 0,
        'FailedCount': 0,
        'InterruptedCount': 0,
        'SucceededCount': 0,
        'TimedOutCount': 0,
        'TotalCount': 0
    }
});

/**
 * Models a jobs-per-hour count.
 *
 * @constructor
 */
var JobsPerHourModel = CollarModel.extend({
    defaults: {
        'Date': null,
        'JobsPerHour': 0,
        'QueueName': null
    }
});

/**
 * Represents a collection of {JobsPerHourModel}s.
 *
 * @constructor
 */
var JobsPerHourCollection = CollarCollection.extend({
    model: JobsPerHourModel,

    /**
     * Replaces this instance's model collection with the given collection.
     *
     * @param {Object} models An object specifying the new model collection.
     * @param {Object} options The options to use when performing the reset.
     * @return {JobsPerHourCollection} This instance.
     */
    reset: function(models, options) {
        return Backbone.Collection.prototype.reset.call(this, models, options);
    }
});

/**
 * Models a jobs-per-worker count.
 *
 * @constructor
 */
var JobsPerWorkerModel = CollarModel.extend({
    defaults: {
        'Count': 0,
        'MachineAddress': null,
        'MachineName': null,
        'Name': null
    }
});

/**
 * Represents a collection of {JobsPerWorkerModel}s.
 *
 * @constructor
 */
var JobsPerWorkerCollection = CollarCollection.extend({
    model: JobsPerWorkerModel,

    /**
     * Replaces this instance's model collection with the given collection.
     *
     * @param {Object} models An object specifying the new model collection.
     * @param {Object} options The options to use when performing the reset.
     * @return {JobsPerWorkerCollection} This instance.
     */
    reset: function(models, options) {
        return Backbone.Collection.prototype.reset.call(this, models, options);
    }
});

/**
 * Models a complete set of stats.
 *
 * @constructor
 */
var StatsModel = CollarModel.extend({
    fragment: 'stats',
    
    /**
     * Initialization.
     *
     * @param {Object} app A set of initial model attribute values.
     * @param {Object} options Initialization options.
     */
    initialize: function(attributes, options) {
        CollarModel.prototype.initialize.call(this, attributes, options);

        this.counts = new CountsModel();
        //this.counts.bind('change', this.change, this);

        this.historyStatusDistant = new HistoryStatusCountsModel();
        //this.historyStatusDistant.bind('change', this.change, this);

        this.historyStatusRecent = new HistoryStatusCountsModel();
        //this.historyStatusRecent.bind('change', this.change, this);

        this.jobsPerHour = new JobsPerHourCollection();
        //this.jobsPerHour.bind('reset', this.change, this);

        this.jobsPerWorker = new JobsPerWorkerCollection();
        //this.jobsPerWorker.bind('reset', this.change, this);
    },

    /**
     * Fires a 'change' event.
     *
    change: function() {
        this.trigger('change', this);
    },*/

    /**
     * Sets the given attributes on this instance.
     *
     * @param {Object} attributes The attributes to set.
     * @param {Object} options The options to use when setting attributes.
     */
    set: function(attributes, options) {
        attributes = attributes || {};
        options = _.extend({silent: false}, options);

        if (attributes.Counts && !options.silent) {
            this.trigger('counts', this, {counts: attributes.Counts});
        }

        if (this.counts) {
            this.counts.set(this.counts.parse(attributes.Counts), {silent: true});
        }

        if (this.historyStatusDistant) {
            this.historyStatusDistant.set(this.historyStatusDistant.parse(attributes.HistoryStatusDistant), {silent: true});
        }

        if (this.historyStatusRecent) {
            this.historyStatusRecent.set(this.historyStatusRecent.parse(attributes.HistoryStatusRecent), {silent: true});
        }

        if (this.jobsPerHour) {
            this.jobsPerHour.reset(this.jobsPerHour.parse(attributes.JobsPerHourByDay), {silent: true});
        }

        if (this.jobsPerWorker) {
            this.jobsPerWorker.reset(this.jobsPerWorker.parse(attributes.JobsPerWorker), {silent: true});
        }

        return CollarModel.prototype.set.call(this, attributes, options);
    },

    /**
     * Gets a copy of the model's attributes.
     *
     * @return {Object} A copy of the model's underlying attributes.
     */
    toJSON: function() {
        return {
            Counts: this.counts.toJSON(),
            HistoryStatusDistant: this.historyStatusDistant.toJSON(),
            HistoryStatusRecent: this.historyStatusRecent.toJSON(),
            JobsPerHourByDay: this.jobsPerHour.toJSON(),
            JobsPerWorker: this.jobsPerWorker.toJSON()
        };
    }
});