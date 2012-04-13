/**
 * Simple counts model.
 */
var Counts = BlueCollarModel.extend({
    defaults: {
        'HistoryCount': 0,
        'QueueCount': 0,
        'ScheduleCount': 0,
        'WorkingCount': 0,
        'WorkerCount': 0
    }
});

/**
 * History status counts model.
 */
var HistoryStatusCounts = BlueCollarModel.extend({
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
 * Jobs per our item model.
 */
var JobsPerHour = BlueCollarModel.extend({
    defaults: {
        'Date': null,
        'JobsPerHour': 0,
        'QueueName': null
    }
});

/**
 * Jobs per hour collection.
 */
var JobsPerHourList = BlueCollarCollection.extend({
    model: JobsPerHour,

    reset: function(models, options) {
        return Backbone.Collection.prototype.reset.call(this, models, options);
    }
});

/**
 * Jobs per worker item model.
 */
var JobsPerWorker = BlueCollarModel.extend({
    defaults: {
        'Count': 0,
        'MachineAddress': null,
        'MachineName': null,
        'Name': null
    }
});

/**
 * Jobs per worker collection.
 */
var JobsPerWorkerList = BlueCollarCollection.extend({
    model: JobsPerWorker,

    reset: function(models, options) {
        return Backbone.Collection.prototype.reset.call(this, models, options);
    }
});

/**
 * Global stats container model.
 */
var Stats = BlueCollarModel.extend({
    counts: new Counts(),
    historyStatusDistant: new HistoryStatusCounts(),
    historyStatusRecent: new HistoryStatusCounts(),
    jobsPerHour: new JobsPerHourList(),
    jobsPerWorker: new JobsPerWorkerList(),

    initialize: function(attributes, options) {
        BlueCollarModel.prototype.initialize.call(this, attributes, options);
        this.counts.bind('change', this.change, this);
        this.historyStatusDistant.bind('change', this.change, this);
        this.historyStatusRecent.bind('change', this.change, this);
        this.jobsPerHour.bind('reset', this.change, this);
        this.jobsPerWorker.bind('reset', this.change, this);
    },

    change: function() {
        this.trigger('change', this);
    },

    set: function(attributes, options) {
        attributes = attributes || {};
        options = _.extend({silent: false}, options);

        if (attributes.Counts && !options.silent) {
            this.trigger('counts', this, attributes.Counts);
        }

        this.counts.set(this.counts.parse(attributes.Counts), {silent: true});
        this.historyStatusDistant.set(this.historyStatusDistant.parse(attributes.HistoryStatusDistant), {silent: true});
        this.historyStatusRecent.set(this.historyStatusRecent.parse(attributes.HistoryStatusRecent), {silent: true});
        this.jobsPerHour.reset(this.jobsPerHour.parse(attributes.JobsPerHourByDay), {silent: true});
        this.jobsPerWorker.reset(this.jobsPerWorker.parse(attributes.JobsPerWorker), {silent: true});

        if (!options.silent) {
            this.change();
        }
    },

    toJSON: function() {
        return {
            Counts: this.counts.toJSON(),
            HistoryStatusDistant: this.historyStatusDistant.toJSON(),
            HistoryStatusRecent: this.historyStatusRecent.toJSON(),
            JobsPerHourByDay: this.jobsPerHour.toJSON(),
            JobsPerWorker: this.jobsPerWorker.toJSON()
        };
    },

    url: function() {
        return this.urlRoot;
    }
});