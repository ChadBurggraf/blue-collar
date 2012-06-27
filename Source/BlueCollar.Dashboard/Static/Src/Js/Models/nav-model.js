/**
 * Models a navigation item.
 *
 * @constructor
 */
var NavModel = Backbone.Model.extend({});

/**
 * Represents a collection of {NavModel}s.
 *
 * @constructor
 */
var NavCollection = Backbone.Collection.extend({
    model: NavModel,

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
            urlRoot = this.urlRoot || '',
            showCounts = !!this.showCounts,
            i = 1,
            prop;

        response = response || {};

        function push(id, name, count, url) {
            count = count !== null && showCounts ? count || 0 : null;
            m.push({id: id.toString(), Name: name, Count: count, Current: !!(current && current.get('Name') === name), Url: urlRoot + url});
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
    }
});