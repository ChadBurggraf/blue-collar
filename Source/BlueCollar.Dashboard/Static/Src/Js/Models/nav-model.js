/**
 * Models a navigation item.
 *
 * @constructor
 */
var NavModel = CollarModel.extend({
    fragment: 'counts'
});

/**
 * Represents a collection of {NavModel}s.
 *
 * @constructor
 */
var NavCollection = CollarCollection.extend({
    fragment: 'counts',
    model: NavModel,

    /**
     * Initialization.
     *
     * @param {Array} models The initial set of models to fill the collection with.
     * @param {Object} options Initialization options.
     */
    initialize: function(models, options) {
        CollarCollection.prototype.initialize.call(this, models, options);
        this.currentName = null;
    },

    /**
     * Gets the current nav item.
     *
     * @return {NavModel} The current nav item, or null if none is current.
     */
    getCurrent: function() {
        var item = this.find(function(item) {
            return item.get('Current');
        });

        if (!item && this.length > 0) {
            this.setCurrent(this.at(0).get('Name'));
            item = this.at(0);
        }

        if (!item && this.currentName) {
            item = new NavModel({Name: this.currentName});
        }

        return item || null;
    },

    /**
     * Parses the collection's data as returned by the server.
     *
     * @param {Object} response The raw response object received from the server.
     * @return {Array} The parsed collection data.
     */
    parse: function(response) {
        var m = [],
            current = this.getCurrent(),
            urlRoot = this.options.urlRoot || '',
            showCounts = !!this.showCounts,
            i = 1,
            prop;

        response = response || {};

        function push(id, name, count, url) {
            count = count !== null && showCounts ? count || 0 : null;

            m.push({
                id: id.toString(), 
                Name: name, 
                Count: count,
                Current: !!(current && current.get('Name') === name), 
                Url: urlRoot + url
            });
        }

        push(i++, 'Dashboard', null, '#dashboard');
        push(i++, 'Working', response['WorkingCount'], '#working');
        push(i++, 'Queue', response['QueueCount'], '#queue');
        push(i++, 'History', response['HistoryCount'], '#history');
        push(i++, 'Workers', response['WorkerCount'], '#workers');
        push(i++, 'Schedules', response['ScheduleCount'], '#schedules');

        if (this.testLink) {
            push(i++, 'Tests', null, 'test?noglobals=true');
        }

        if (!current) {
            m[0].Current = true;
        }

        return m;
    },

    /**
     * Replaces this instance's model collection with the given collection.
     *
     * @param {Object} models An object specifying the new model collection.
     * @param {Object} options The options to use when performing the reset.
     * @return {CollarCollection} This instance.
     */
    reset: function(models, options) {
        return Backbone.Collection.prototype.reset.call(this, models, options);
    },

    /**
     * Sets the nav item with the given name to be the current nav item.
     *
     * @param {String} name The name of the nav item to set current.
     */
    setCurrent: function(name) {
        var item,
            i,
            n;

        for (i = 0, n = this.length; i < n; i++) {
            item = this.at(i);

            if (item.get('Name') === name) {
                item.set({Current: true});
            } else {
                item.set({Current: false});
            }
        }

        this.currentName = name;
    }
});